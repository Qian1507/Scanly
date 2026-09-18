# Scanly AB – Full Cloud Solution

## Project Overview

## Team

## System Architecture

## Repository Structure

## Development Workflow

Our Git workflow is:

`main ← dev ← task branches`

For each Azure DevOps Task:

1. Switch to `dev`.
2. Pull the latest changes from `origin/dev`.
3. Create a new Task branch from the updated `dev` branch.
4. Implement and test the Task.
5. Commit the changes with the Azure DevOps Task ID.
6. Push the Task branch.
7. Create a Pull Request to `dev`.
8. Get at least one review from another team member.
9. Merge the Pull Request and verify the result.

Example:

```bash
git switch dev
git pull origin dev
git switch -c task-16-create-scanly-api

### Branch Naming

Format:

`task-<task-id>-<short-description>`

Example:

`task-15-repository-structure`

### Commit Convention

Format:

`AB#<task-id> <description>`

Example:

`AB#15 Create initial repository structure`

`AB#15` links the commit to the corresponding Azure DevOps Work Item.

The `dev` branch is used for ongoing development, while `main` contains the stable version used for final deployment.

## CI/CD Workflow

## Local Development

## Docker

## Azure Infrastructure

## Configuration and Security

## API Endpoints

## Documentation

## Project Status