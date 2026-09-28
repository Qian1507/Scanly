# Scanly AB – Full Cloud Solution

Scanly is a cloud-based .NET solution for invoice processing. The API receives invoice files, analyzes them with Azure AI Document Intelligence, maps the extracted data to Scanly-specific models, and stores the result as JSON in Azure Blob Storage.

The application is containerized with Docker and deployed to Azure Container Apps in the development environment. Azure DevOps is used for Work Items, Git branches, Pull Requests, CI/CD, automated testing, and team collaboration.

> The production configuration was validated with Bicep `what-if`, but a separate production environment was not deployed as part of this project.

## Features

- Invoice upload through an ASP.NET Core API.
- Invoice analysis with Azure AI Document Intelligence.
- Mapping of extracted data to Scanly-specific models.
- JSON persistence in Azure Blob Storage.
- Docker containerization.
- Azure Container Registry image storage.
- Azure Container Apps hosting.
- Bicep Infrastructure as Code.
- Managed Identity and RBAC for Azure resource access.
- Automated API tests.
- Azure DevOps CI/CD.
- Development and intended production configuration through separate Bicep parameter files.

## Technology Stack

- .NET and ASP.NET Core.
- Azure AI Document Intelligence.
- Azure Blob Storage.
- Azure Container Registry.
- Azure Container Apps.
- Azure DevOps.
- Docker.
- Bicep.
- Git and Pull Requests.

## Team and Development Workflow

The project was developed by three students. Azure DevOps Work Items were used to organize the work.

The branch structure is:

```text
main ← dev ← task branches
```

For each task, the normal workflow is:

1. Switch to `dev` and pull the latest changes.
2. Create a task branch.
3. Implement and test the change.
4. Commit using the related Azure DevOps Task ID.
5. Push the branch.
6. Create a Pull Request to `dev`.
7. Obtain at least one review.
8. Merge the Pull Request.

### Branch Naming

```text
task-<task-id>-<short-description>
```

Example:

```text
task-16-create-scanly-api
```

### Commit Convention

```text
AB#<task-id> <description>
```

Example:

```text
AB#15 Create initial repository structure
```

The `dev` branch is used for ongoing integration. The `main` branch contains the stable version and triggers the deployment workflow.

## API Endpoints

| Method | Endpoint | Description |
| --- | --- | --- |
| GET | `/health` | Verifies that the API is running. |
| POST | `/invoices` | Uploads and processes an invoice. |
| GET | `/invoices/{id}` | Retrieves one stored invoice result. |
| GET | `/invoices` | Retrieves stored invoice results. |

`POST /invoices` accepts a PDF or image invoice, sends it to Azure AI Document Intelligence, maps the analysis result to the Scanly response model, and stores the result in Azure Blob Storage.

## Prerequisites

Install the following tools before running the project locally:

- .NET SDK compatible with the project.
- Docker Desktop, if Docker execution is required.
- Azure CLI, if Azure deployment or Bicep validation is required.
- Access to the course-provided Azure AI Document Intelligence resource.
- Access to the required Azure subscription and resource group for deployment.

## Local Development

The API uses environment variables for Azure services.

Required values include:

```text
AZURE_DI_ENDPOINT
AZURE_DI_KEY
AZURE_STORAGE_URL
AZURE_STORAGE_CONTAINER
```

A local PowerShell script can be used to load the development environment:

```powershell
. .\local-env.ps1
```

`local-env.ps1` is ignored by Git and must not be committed because it may contain sensitive values.

Restore dependencies when required:

```powershell
dotnet restore
```

Start the API from the repository root:

```powershell
dotnet run --project Scanly.Api
```

Swagger is available at the URL shown in the terminal output. It normally follows this format:

```text
http://localhost:<port>/swagger
```

> Do not commit Azure keys, connection strings, local environment files, or other sensitive values to Git.

## Automated Tests

Run the API tests with:

```powershell
dotnet test Scanly.Api.Tests/Scanly.Api.Tests.csproj
```

The endpoint tests use fake invoice analysis and storage services. Therefore, CI tests do not require live access to Azure AI Document Intelligence or Azure Blob Storage.

The test suite covers:

```text
GET  /health
POST /invoices
GET  /invoices/{id}
GET  /invoices/{id} -> 404
GET  /invoices
```

## Run with Docker

Build the Docker image from the repository root:

```powershell
docker build -t scanly-api .
```

Run the container:

```powershell
docker run --rm -p 8080:8080 scanly-api
```

Verify the health endpoint:

```text
http://localhost:8080/health
```

Swagger is available at:

```text
http://localhost:8080/swagger
```

To run the container with Azure service configuration, pass the required environment variables:

```powershell
docker run --rm -p 8080:8080 `
  -e AZURE_DI_ENDPOINT="$env:AZURE_DI_ENDPOINT" `
  -e AZURE_DI_KEY="$env:AZURE_DI_KEY" `
  -e AZURE_STORAGE_URL="$env:AZURE_STORAGE_URL" `
  -e AZURE_STORAGE_CONTAINER="$env:AZURE_STORAGE_CONTAINER" `
  scanly-api
```

Do not store Azure credentials in the Dockerfile or commit them to Git.

## Infrastructure as Code

Azure infrastructure is defined with Bicep.

Main infrastructure files:

```text
infra/
├── bootstrap.bicep
├── main.bicep
├── dev.bicepparam
└── prod.bicepparam
```

`bootstrap.bicep` creates or updates Azure Container Registry before the Docker image is pushed.

`main.bicep` deploys and configures the remaining infrastructure, including:

- Storage Account and Blob container.
- Container Apps Environment.
- Container App.
- Managed Identity.
- ACR pull access.
- Blob Storage RBAC.
- Application configuration and secrets.
- HTTP-based autoscaling.

The same `main.bicep` template is reused for the development environment and the intended production configuration through separate parameter files.

## Environment Configuration

Development configuration:

```text
environment = dev
minReplicas = 1
maxReplicas = 2
```

Intended production configuration:

```text
environment = prod
minReplicas = 2
maxReplicas = 5
```

The development environment was deployed and validated.

The production configuration was validated with Azure CLI and Bicep `what-if`, but no separate production environment was deployed and no production resources were created as part of this project.

Example validation commands:

```powershell
az deployment group what-if `
  --resource-group $rg `
  --parameters infra/dev.bicepparam
```

```powershell
az deployment group what-if `
  --resource-group $rg `
  --parameters infra/prod.bicepparam
```

Sensitive deployment values must be supplied through local environment variables or secure Azure DevOps variables instead of being stored directly in parameter files.

## CI/CD

The final branch strategy is:

```text
dev  -> CI only
main -> CI + CD
```

The deployment workflow was used for the deployed development environment. The production parameter configuration was reviewed with `what-if` only and was not deployed.

### Continuous Integration

CI includes:

- Restore .NET dependencies.
- Build the API.
- Build the test project.
- Run automated tests.

### Continuous Deployment

The deployment workflow includes:

```text
Deploy bootstrap.bicep
        ↓
Build Docker image
        ↓
Tag image with Azure DevOps Build ID
        ↓
Push image to ACR
        ↓
Deploy main.bicep
        ↓
Update Container App
        ↓
Verify /health
```

Docker images are tagged with the Azure DevOps Build ID so that each deployment has a traceable image version.

The pipeline uses an Azure service connection and secure Azure DevOps variables for deployment access.

## Security

Azure Blob Storage is accessed using:

- `DefaultAzureCredential`.
- The Container App system-assigned Managed Identity.
- The `Storage Blob Data Contributor` role.

The Container App uses its Managed Identity to pull images from ACR through the `AcrPull` role.

Storage Account keys and connection strings are not used by the application.

The Azure AI Document Intelligence key is supplied through local environment variables and secure Azure DevOps or Container App configuration.

Sensitive values must not be hard-coded in application source code, Dockerfiles, Bicep parameter files, or committed local environment scripts.

## Documentation

Detailed system structure, deployment design, security, scaling, and technical decisions are documented in:

```text
ARCHITECTURE.md
```

Project background, development process, testing, results, limitations, and future improvements are documented in:

```text
RAPPORT.md
```

## Project Status

The development environment includes the following implemented and validated capabilities:

- Working invoice API.
- Azure AI Document Intelligence integration.
- Azure Blob Storage persistence.
- Managed Identity and RBAC.
- Docker containerization.
- Azure Container Registry.
- Azure Container Apps.
- Bicep Infrastructure as Code.
- Development parameterization and deployment.
- Intended production parameter configuration validated with `what-if`.
- HTTP-based autoscaling configuration.
- Automated API tests.
- Azure DevOps CI/CD.
- Deployment health verification.

A separate production environment was not deployed. As required by the course, the production configuration was validated with Bicep `what-if` only.