<#
.SYNOPSIS
Deploys the application locally to the staging slot, verifies health, and swaps it into production.

.EXAMPLE
./scripts/Deploy-StagingThenSwap.ps1 -ResourceGroupName rg-robgit -WebAppName ropgithub-web-12345
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)]
    [ValidateNotNullOrEmpty()]
    [string]$ResourceGroupName,

    [Parameter(Mandatory)]
    [ValidatePattern('^[a-z0-9][a-z0-9-]{0,58}[a-z0-9]$')]
    [string]$WebAppName,

    [ValidateSet('eastus2')]
    [string]$Location = 'eastus2',

    [ValidateSet('S1', 'S2', 'S3', 'P0v3', 'P1v3', 'P2v3', 'P3v3')]
    [string]$AppServicePlanSku = 'S1',

    [ValidateRange(1, 60)]
    [int]$HealthCheckAttempts = 20,

    [ValidateRange(1, 60)]
    [int]$HealthCheckDelaySeconds = 10
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

function Invoke-AzCli {
    param([Parameter(Mandatory)][string[]]$Arguments)

    & az @Arguments
    if ($LASTEXITCODE -ne 0) {
        throw "Azure CLI command failed: az $($Arguments -join ' ')"
    }
}

function Test-HealthEndpoint {
    param([Parameter(Mandatory)][string]$Url)

    for ($attempt = 1; $attempt -le $HealthCheckAttempts; $attempt++) {
        try {
            $response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 30
            if ($response.StatusCode -ge 200 -and $response.StatusCode -lt 300) {
                Write-Host "Health check passed for $Url (HTTP $($response.StatusCode))."
                return
            }
        }
        catch {
            Write-Host "Health check attempt $attempt/$HealthCheckAttempts for $Url failed: $($_.Exception.Message)"
        }

        if ($attempt -lt $HealthCheckAttempts) {
            Start-Sleep -Seconds $HealthCheckDelaySeconds
        }
    }

    throw "Health check did not succeed for $Url after $HealthCheckAttempts attempts. The staging slot was not swapped."
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$templateFile = Join-Path $repoRoot 'infra/main.bicep'
$projectFile = Join-Path $repoRoot 'RobGitHub.Web/RobGitHub.Web.csproj'
$publishDirectory = Join-Path $repoRoot '.artifacts/publish'
$packageFile = Join-Path $repoRoot '.artifacts/RobGitHub.Web.zip'

if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI is required. Install it from https://learn.microsoft.com/cli/azure/install-azure-cli.'
}

if (-not (Test-Path $templateFile) -or -not (Test-Path $projectFile)) {
    throw 'Run this script from the repository checkout; deployment files or project file were not found.'
}

Invoke-AzCli @('account', 'show', '--output', 'none')
Invoke-AzCli @('group', 'create', '--name', $ResourceGroupName, '--location', $Location, '--output', 'none')
Invoke-AzCli @(
    'deployment', 'group', 'create',
    '--resource-group', $ResourceGroupName,
    '--template-file', $templateFile,
    '--parameters', "webAppName=$WebAppName", "location=$Location", "appServicePlanSku=$AppServicePlanSku",
    '--output', 'none'
)

Remove-Item $publishDirectory -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item $packageFile -Force -ErrorAction SilentlyContinue
New-Item -ItemType Directory -Path $publishDirectory -Force | Out-Null

dotnet publish $projectFile --configuration Release --output $publishDirectory
if ($LASTEXITCODE -ne 0) {
    throw 'dotnet publish failed.'
}

Compress-Archive -Path (Join-Path $publishDirectory '*') -DestinationPath $packageFile -CompressionLevel Optimal
Invoke-AzCli @(
    'webapp', 'deploy',
    '--resource-group', $ResourceGroupName,
    '--name', $WebAppName,
    '--slot', 'staging',
    '--src-path', $packageFile,
    '--type', 'zip',
    '--track-status', 'true',
    '--output', 'none'
)

$stagingUrl = "https://$WebAppName-staging.azurewebsites.net/health"
$productionUrl = "https://$WebAppName.azurewebsites.net/health"
Test-HealthEndpoint -Url $stagingUrl

Invoke-AzCli @(
    'webapp', 'deployment', 'slot', 'swap',
    '--resource-group', $ResourceGroupName,
    '--name', $WebAppName,
    '--slot', 'staging',
    '--target-slot', 'production',
    '--output', 'none'
)

Test-HealthEndpoint -Url $productionUrl
Write-Host "Deployment completed successfully. Production endpoint: $productionUrl"
