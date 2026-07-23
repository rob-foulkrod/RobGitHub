# RobGitHub

A fun-looking .NET 10 MVC todo application with:

- in-memory todo storage
- xUnit test coverage
- a `/health` endpoint
- on-screen pending todo notifications

## Run

```powershell
dotnet run --project .\RobGitHub.Web
```

## Test

```powershell
dotnet test .\RobGitHub.slnx
```

## Azure deployment

The Bicep template in `infra/main.bicep` creates a Linux App Service plan and a web app in East US 2 with a `staging` deployment slot. The default `S1` plan is required because deployment slots are not available in Free, Shared, or Basic plans.

On a push to `main`, GitHub Actions deploys the Bicep template, publishes and packages the app, deploys it to `staging` with `azure/webapps-deploy`, verifies `/health`, swaps staging into production, and verifies production health. The swap is not attempted when staging is unhealthy.

### GitHub Actions setup

The workflow at `.github/workflows/ci-cd.yml` runs restore, format verification, build, tests, coverage enforcement, a vulnerable-dependency scan, application publish, and Bicep compilation in CI. CI uploads the validated application ZIP and compiled ARM template as artifacts. On a push to `main`, CD downloads those exact artifacts and deploys them through GitHub Actions; CD does not rebuild source code or recompile Bicep.

Configure these GitHub Actions repository secrets before merging to `main`: `AZURE_CLIENT_ID`, `AZURE_TENANT_ID`, `AZURE_SUBSCRIPTION_ID`, `AZURE_RESOURCE_GROUP`, and `AZURE_WEBAPP_NAME`.

The deployment identity is a user-assigned managed identity. It must have a federated credential that trusts this repository's `main` branch and `Contributor` access to the deployment resource group.
