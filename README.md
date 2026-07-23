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

