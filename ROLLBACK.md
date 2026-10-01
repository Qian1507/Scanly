# Scanly API Rollback Procedure

This document describes how to roll back the Scanly API in Azure Container Apps to a previously working revision.

## When to perform a rollback

A rollback should be performed when a newly deployed revision:

- Fails its startup, readiness, or liveness probes.
- Returns errors from the `/health` endpoint.
- Causes unexpected application behaviour.
- Cannot serve production traffic correctly.

## Prerequisites

Before starting, verify that:

- Azure CLI is installed.
- You are signed in with `az login`.
- The correct Azure subscription is selected.
- The Container App uses multiple revision mode.
- A previous healthy revision is available.

## Azure resources

- Container App: `scanly-dev-app`
- Resource group: `RG-Zara-Rangkhoni-e8ef10-DotNetCloudDeveloper-VT-Mars-Goteborg`
- Health endpoint: `https://scanly-dev-app.nicesand-27d81a3b.swedencentral.azurecontainerapps.io/health`

## 1. List available revisions

Run:

```powershell
az containerapp revision list `
  --name scanly-dev-app `
  --resource-group RG-Zara-Rangkhoni-e8ef10-DotNetCloudDeveloper-VT-Mars-Goteborg `
  --output table
```

Identify the latest known healthy revision before changing traffic.

## 2. Route traffic to the previous healthy revision

Replace the revision names when necessary.

```powershell
az containerapp ingress traffic set `
  --name scanly-dev-app `
  --resource-group RG-Zara-Rangkhoni-e8ef10-DotNetCloudDeveloper-VT-Mars-Goteborg `
  --revision-weight scanly-dev-app--0000002=100 scanly-dev-app--rollback-demo=0
```

This routes 100 percent of incoming traffic to the previous healthy revision and removes traffic from the new revision.

## 3. Verify the traffic allocation

Run:

```powershell
az containerapp ingress traffic show `
  --name scanly-dev-app `
  --resource-group RG-Zara-Rangkhoni-e8ef10-DotNetCloudDeveloper-VT-Mars-Goteborg `
  --output table
```

Confirm that the healthy revision receives 100 percent of the traffic.

## 4. Verify application health

Run:

```powershell
Invoke-WebRequest `
  -UseBasicParsing `
  -Uri "https://scanly-dev-app.nicesand-27d81a3b.swedencentral.azurecontainerapps.io/health" |
  Select-Object StatusCode, Content
```

Expected response:

```text
StatusCode: 200
Content: {"status":"healthy"}
```

## Tested rollback

The rollback procedure was tested on 28 September 2026.

A new healthy test revision named `scanly-dev-app--rollback-demo` was created and temporarily received 100 percent of the application traffic. The traffic was then moved back to the previously verified revision `scanly-dev-app--0000002`.

After the rollback:

- `scanly-dev-app--0000002` received 100 percent of the traffic.
- The `/health` endpoint returned HTTP status code `200`.
- The response was `{"status":"healthy"}`.
- The application remained available during the traffic change.

## Important notes

- Do not delete the failed revision until the cause of the problem has been investigated.
- Always verify the health endpoint after changing traffic.
- Confirm the resource group and revision names before running the rollback command.
- Record the rollback reason and selected revision in the deployment documentation.