targetScope = 'resourceGroup'

@description('Azure region used for all resources')
param location string = resourceGroup().location

@description('Project name used as a resource naming prefix')
param projectName string = 'scanly'

@description('Environment name, for example dev or prod')
param environment string = 'dev'

@description('Name of the blob container')
param blobContainerName string = 'invoices'

var namePrefix = '${projectName}-${environment}'

// Storage Account and Azure Container Registry names
// cannot contain hyphens and must be globally unique.
var uniqueSuffix = uniqueString(resourceGroup().id)
var storageAccountName = take('${projectName}${environment}${uniqueSuffix}', 24)
var containerRegistryName = take('${projectName}${environment}${uniqueSuffix}', 50)


// --------------------------------------------------
// Azure Container Registry
// --------------------------------------------------

resource containerRegistry 'Microsoft.ContainerRegistry/registries@2023-07-01' = {
  name: containerRegistryName
  location: location

  sku: {
    name: 'Basic'
  }

  properties: {
    adminUserEnabled: false
  }
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

      ingress: {
        external: true
        targetPort: 80
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
          image: 'mcr.microsoft.com/azuredocs/containerapps-helloworld:latest'

          resources: {
            cpu: json('0.5')
            memory: '1Gi'
          }
        }
      ]

      scale: {
        minReplicas: 0
        maxReplicas: 2
      }
    }
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