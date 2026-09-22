targetScope = 'resourceGroup'

@description('Azure region used for all resources')
param location string = resourceGroup().location

@description('Project name used as a resource naming prefix')
param projectName string = 'scanly'

@description('Environment name, for example dev or prod')
param environment string = 'dev'

var namePrefix = '${projectName}-${environment}'

// Azure resources will be added in later tasks.