targetScope = 'subscription'

param basename string
param location string = 'westus2'
param failoverLocation string = 'eastus2'
param principalId string

var resourceGroupName = '${basename}rg'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: resourceGroupName
  location: location
}

module resources './resources.bicep' = {
  name: 'resources-${resourceGroupName}'
  scope: resourceGroup(rg.name)
  params: {
    basename: basename
    location: location
    failoverLocation: failoverLocation
    principalId: principalId
  }
}