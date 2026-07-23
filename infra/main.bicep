targetScope = 'resourceGroup'

@description('Globally unique name for the Azure App Service web app.')
@minLength(2)
@maxLength(60)
param webAppName string

@description('Azure region for all resources. This deployment is intended for East US 2.')
param location string = 'eastus2'

@description('SKU name for the Linux App Service plan. Standard or higher is required for deployment slots.')
@allowed([
  'S1'
  'S2'
  'S3'
  'P0v3'
  'P1v3'
  'P2v3'
  'P3v3'
])
param appServicePlanSku string = 'S1'

@description('Health-check endpoint exposed by the application.')
param healthCheckPath string = '/health'

var appServicePlanName = '${webAppName}-plan'
var appServicePlanTier = startsWith(appServicePlanSku, 'P') ? 'PremiumV3' : 'Standard'
var commonSiteConfig = {
  alwaysOn: true
  ftpsState: 'Disabled'
  healthCheckPath: healthCheckPath
  http20Enabled: true
  linuxFxVersion: 'DOTNETCORE|10.0'
  minTlsVersion: '1.2'
}

resource appServicePlan 'Microsoft.Web/serverfarms@2024-04-01' = {
  name: appServicePlanName
  location: location
  kind: 'linux'
  sku: {
    name: appServicePlanSku
    tier: appServicePlanTier
  }
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2024-04-01' = {
  name: webAppName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: appServicePlan.id
    siteConfig: union(commonSiteConfig, {
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Production'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    })
  }
}

resource stagingSlot 'Microsoft.Web/sites/slots@2024-04-01' = {
  parent: webApp
  name: 'staging'
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    httpsOnly: true
    serverFarmId: appServicePlan.id
    siteConfig: union(commonSiteConfig, {
      appSettings: [
        {
          name: 'ASPNETCORE_ENVIRONMENT'
          value: 'Staging'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: '1'
        }
      ]
    })
  }
}

@description('Marks environment configuration as sticky so it remains attached to its slot after a swap.')
resource slotConfigNames 'Microsoft.Web/sites/config@2024-04-01' = {
  parent: webApp
  name: 'slotConfigNames'
  properties: {
    appSettingNames: [
      'ASPNETCORE_ENVIRONMENT'
    ]
  }
}

output webAppName string = webApp.name
output productionUrl string = 'https://${webApp.properties.defaultHostName}'
output stagingUrl string = 'https://${stagingSlot.properties.defaultHostName}'
