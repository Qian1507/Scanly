# Scanly AB – Full Cloud Solution

## Project Overview

Scanly AB is a .NET cloud project for invoice processing.

The API receives invoice files and will use Azure AI Document Intelligence to extract invoice data.

Azure DevOps is used for Tasks, Git branches, Pull Requests, and team collaboration.

## Team

The project is developed by three students.

Each Azure DevOps Task is implemented in a separate branch and merged into `dev` through a Pull Request.

## Development Workflow

Git workflow:

```text
main ← dev ← task branches
```

For each Task:

1. Switch to `dev`
2. Pull latest changes
3. Create a Task branch
4. Implement and test
5. Commit with Task ID
6. Push branch
7. Create Pull Request to `dev`
8. Get one review
9. Merge

Example:

```bash
git switch dev
git pull origin dev
git switch -c task-16-create-scanly-api
```

### Branch Naming

Format:

```text
task-<task-id>-<short-description>
```

Example:

```text
task-15-repository-structure
```

### Commit Convention

Format:

```text
AB#<task-id> <description>
```

Example:

```text
AB#15 Create initial repository structure
```

`AB#<task-id>` links the commit to the corresponding Azure DevOps Work Item.

The `dev` branch is used for ongoing development, while `main` contains the stable version.

## Local Development

The API uses local environment variables for Azure Document Intelligence.

Required variables:

```text
AZURE_DI_ENDPOINT
AZURE_DI_KEY
```

Example in PowerShell:

```powershell
$env:AZURE_DI_ENDPOINT="..."
$env:AZURE_DI_KEY="..."
```

Then run the API from the repository root:

```powershell
dotnet run --project Scanly.Api
```

Open Swagger in the browser:

```text
http://localhost:<port>/swagger
```

Use `POST /invoices` to upload a PDF or image invoice for analysis.

The Azure Document Intelligence endpoint and key are provided in the course material.

> Sensitive values must not be committed to Git.

## Project Status

The project is currently under development.