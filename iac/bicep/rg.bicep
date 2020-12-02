targetScope = 'subscription'

param basename string = ''
param location string = 'westus2'
param failover_location string = 'eastus2'
param cli_user_id string = ''

var resource_group_name = '${basename}rg'

resource rg 'Microsoft.Resources/resourceGroups@2020-06-01' = {
  name: resource_group_name
  location: location
}

module resources './resources.bicep' = {
  name: 'resources'
  scope: resourceGroup(rg.name)
  params: {
    basename: basename
    location: location
    failover_location: failover_location
    cli_user_id: cli_user_id
  }
}