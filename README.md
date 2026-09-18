# Scanly AB – Full Cloud Solution

## Project Overview

## Team

## System Architecture

## Repository Structure

## Development Workflow

Our Git workflow is:

`main ← dev ← task branches`

For each Azure DevOps Task:

1. Update the local `dev` branch.
2. Create a new branch from `dev`.
3. Implement and test the Task.
4. Commit the changes with the Azure DevOps Task ID.
5. Push the Task branch.
6. Create a Pull Request to `dev`.
7. Get at least one review from another team member.
8. Merge the Pull Request and verify the result.

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