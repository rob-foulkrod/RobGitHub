# RobGitHub

A fun-looking .NET 10 MVC todo application with:

- in-memory todo storage
- xUnit test coverage
- a `/health` endpoint
- on-screen pending todo notifications

## Run

```bash
dotnet run --project /home/runner/work/RobGitHub/RobGitHub/RobGitHub.Web
```

## Test

```bash
dotnet test /home/runner/work/RobGitHub/RobGitHub/RobGitHub.slnx
```

## Azure deployment

The Bicep template in `infra/main.bicep` creates a Linux App Service plan and a web app in East US 2 with a `staging` deployment slot. The default `S1` plan is required because deployment slots are not available in Free, Shared, or Basic plans.

From an authenticated Azure CLI session, deploy locally to staging, validate `/health`, and swap the slot into production:

```powershell
./scripts/Deploy-StagingThenSwap.ps1 `
	-ResourceGroupName '<resource-group-name>' `
	-WebAppName '<globally-unique-web-app-name>'
```

The script creates the resource group if necessary, deploys the Bicep template, publishes and ZIP-deploys the app to `staging`, verifies the staging health endpoint, swaps staging into production, and verifies production health. It stops before swapping when staging is unhealthy.

### GitHub Actions setup

The workflow at `.github/workflows/ci-cd.yml` runs restore, format verification, build, tests, and a vulnerable-dependency scan on every push. Pushes to `main` then deploy through the same PowerShell script.

Configure these GitHub Actions repository secrets before merging to `main`: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `AZURE_RESOURCE_GROUP`, and `AZURE_WEBAPP_NAME`.

The deployment identity is a user-assigned managed identity. It must have a federated credential that trusts this repository's `main` branch and `Contributor` access to the deployment resource group.
