using './main.bicep'

param environment = 'prod'
param minReplicas = 2
param maxReplicas = 5
param concurrentRequests = '10'
param blobContainerName = 'invoices'



param containerRegistryName = readEnvironmentVariable('AZURE_ACR_NAME')
param diEndpoint = readEnvironmentVariable('AZURE_DI_ENDPOINT')
param diKey = readEnvironmentVariable('AZURE_DI_KEY')

param imageTag = 'latest'