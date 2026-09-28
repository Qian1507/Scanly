targetScope = 'resourceGroup'

// --------------------------------------------------
// Parameters
// --------------------------------------------------

@description('Container image tag')
param imageTag string = 'latest'

@description('Azure Container Registry name created by bootstrap.bicep')
param containerRegistryName string

@description('Azure Document Intelligence endpoint')
param diEndpoint string

@secure()
@description('Azure Document Intelligence key')
param diKey string

@description('Azure region used for all resources')
param location string = resourceGroup().location

@description('Project name used as a resource naming prefix')
param projectName string = 'scanly'

@description('Environment name, for example dev or prod')
param environment string = 'dev'

@description('Name of the blob container')
param blobContainerName string = 'invoices'

// --------------------------------------------------
// Variables
// --------------------------------------------------

var namePrefix = '${projectName}-${environment}'

// Storage Account names cannot contain hyphens
// and must be globally unique.
var uniqueSuffix = uniqueString(resourceGroup().id)
var storageAccountName = take('${projectName}${environment}${uniqueSuffix}', 24)

// Built-in Azure RBAC roles
var acrPullRoleId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  '7f951dda-4ed3-4680-a7ca-43fe172d538d'
)

var storageBlobDataContributorRoleId = subscriptionResourceId(
  'Microsoft.Authorization/roleDefinitions',
  'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
)

// --------------------------------------------------
// Existing Azure Container Registry
// --------------------------------------------------

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' existing = {
  name: containerRegistryName
}

// --------------------------------------------------
// Storage Account
// --------------------------------------------------

resource storageAccount 'Microsoft.Storage/storageAccounts@2023-05-01' = {
  name: storageAccountName
  location: location

  sku: {
    name: 'Standard_LRS'
  }

  kind: 'StorageV2'

  properties: {
    allowBlobPublicAccess: false
    minimumTlsVersion: 'TLS1_2'
    supportsHttpsTrafficOnly: true
  }
}

// --------------------------------------------------
// Blob Service
// --------------------------------------------------

resource blobService 'Microsoft.Storage/storageAccounts/blobServices@2023-05-01' = {
  parent: storageAccount
  name: 'default'
}

// --------------------------------------------------
// Blob Container
// --------------------------------------------------

resource blobContainer 'Microsoft.Storage/storageAccounts/blobServices/containers@2023-05-01' = {
  parent: blobService
  name: blobContainerName

  properties: {
    publicAccess: 'None'
  }
}

// --------------------------------------------------
// Container Apps Environment
// --------------------------------------------------

resource containerAppsEnvironment 'Microsoft.App/managedEnvironments@2024-03-01' = {
  name: '${namePrefix}-environment'
  location: location

  properties: {}
}

// --------------------------------------------------
// Container App
// --------------------------------------------------

resource containerApp 'Microsoft.App/containerApps@2024-03-01' = {
  name: '${namePrefix}-app'
  location: location

  identity: {
    type: 'SystemAssigned'
  }

  properties: {
    managedEnvironmentId: containerAppsEnvironment.id

    configuration: {
      activeRevisionsMode: 'Single'

      // Use the Container App system-assigned identity
      // to authenticate against ACR.
      registries: [
        {
          server: containerRegistry.properties.loginServer
          identity: 'system'
        }
      ]

      // Document Intelligence key is stored as a secret.
      secrets: [
        {
          name: 'azure-di-key'
          value: diKey
        }
      ]

      ingress: {
        external: true
        targetPort: 8080
        allowInsecure: false

        traffic: [
          {
            latestRevision: true
            weight: 100
          }
        ]
      }
    }

    template: {
      containers: [
        {
          name: 'scanly-api'
          image: '${containerRegistry.properties.loginServer}/scanly-api:${imageTag}'

          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }

          env: [
            {
              name: 'AZURE_DI_ENDPOINT'
              value: diEndpoint
            }
            {
              name: 'AZURE_DI_KEY'
              secretRef: 'azure-di-key'
            }
            {
              name: 'AZURE_STORAGE_URL'
              value: 'https://${storageAccount.name}.${az.environment().suffixes.storage}'
            }
            {
              name: 'AZURE_STORAGE_CONTAINER'
              value: blobContainer.name
            }
          ]
        }
      ]

      scale: {
  minReplicas: 1
  maxReplicas: 5

  rules: [
    {
      name: 'http-scaling-rule'
      http: {
        metadata: {
          concurrentRequests: '10'
        }
      }
    }
  ]
}
    }
  }
}

// --------------------------------------------------
// ACR Pull RBAC
// --------------------------------------------------

resource acrPullRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(
    containerRegistry.id,
    containerApp.id,
    acrPullRoleId
  )

  scope: containerRegistry

  properties: {
    roleDefinitionId: acrPullRoleId
    principalId: containerApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// --------------------------------------------------
// Blob Storage RBAC
// --------------------------------------------------

resource storageRoleAssignment 'Microsoft.Authorization/roleAssignments@2022-04-01' = {
  name: guid(
    storageAccount.id,
    containerApp.id,
    storageBlobDataContributorRoleId
  )

  scope: storageAccount

  properties: {
    roleDefinitionId: storageBlobDataContributorRoleId
    principalId: containerApp.identity.principalId
    principalType: 'ServicePrincipal'
  }
}

// --------------------------------------------------
// Outputs
// --------------------------------------------------

output containerRegistryName string = containerRegistry.name
output containerRegistryLoginServer string = containerRegistry.properties.loginServer
output storageAccountName string = storageAccount.name
output blobContainerName string = blobContainer.name
output containerAppsEnvironmentName string = containerAppsEnvironment.name
output containerAppName string = containerApp.name
output containerAppUrl string = 'https://${containerApp.properties.configuration.ingress.fqdn}'