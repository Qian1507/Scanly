targetScope = 'resourceGroup'

// --------------------------------------------------
// Parameters
// --------------------------------------------------

@description('Azure region used for all resources')
param location string = resourceGroup().location

@description('Project name used as a resource naming prefix')
param projectName string = 'scanly'

@description('Environment name, for example dev or prod')
param environment string = 'dev'

// --------------------------------------------------
// Variables
// --------------------------------------------------

// Azure Container Registry names cannot contain hyphens
// and must be globally unique.
var uniqueSuffix = uniqueString(resourceGroup().id)
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
// Outputs
// --------------------------------------------------

output containerRegistryName string = containerRegistry.name
output containerRegistryLoginServer string = containerRegistry.properties.loginServer