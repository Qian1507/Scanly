# Scanly AB – Customer Report


## Infrastructure deployment verification

The Bicep infrastructure was deployed successfully to the existing Azure resource group.

Verified resources:

- Azure Container Registry
- Storage Account
- Private Blob container named `invoices`
- Azure Container Apps Environment
- Azure Container App
- System-assigned Managed Identity

The deployment completed with the provisioning state `Succeeded`.
All resources were created in the `swedencentral` region.

## Customer Problem

## Proposed Solution

## Architecture

## Security

## Deployment

On 25 September 2026, the Docker image `scanly-api:v1` was verified locally and deployed from Azure Container Registry to Azure Container Apps.

The first deployment did not return the Scanly API. The Container App initially displayed the default Azure page, and the new revision later failed its startup probes. Log inspection showed that the ingress target port and the TCP startup, readiness, and liveness probes were configured for port 80, while the ASP.NET Core application listens on port 8080.

The target port and all health probe ports were therefore changed to 8080. A new healthy revision, `scanly-dev-app--0000002`, was created, and 100 percent of the traffic was routed to this revision.

Deployed image:

`scanlydev3huikt7fujnc6.azurecr.io/scanly-api:v1`

Public URL:

`https://scanly-dev-app.nicesand-27d81a3b.swedencentral.azurecontainerapps.io`

## Results

The deployed Scanly API was tested through the public HTTPS endpoint.

- Container App revision: `scanly-dev-app--0000002`
- Revision health status: Healthy
- Traffic allocation: 100%
- `/health`: HTTP 200 with response `{"status":"healthy"}`
- `/swagger`: HTTP 200 and redirected successfully to `/swagger/index.html`

The deployment and verification were completed successfully.

The port configuration must also be updated to 8080 in the Bicep infrastructure file to ensure that future deployments do not restore the incorrect port 80 configuration.