param principalType string = 'ServicePrincipal'
param principalId string = ''
param resourceGroupName string = ''
param location string = ''

module blob_contrib './role.bicep' = {
  name: 'blob_contrib-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
    resourceGroupName: resourceGroupName
  }
}

module queue_contrib './role.bicep' = {
  name: 'queue_contrib-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
    resourceGroupName: resourceGroupName
  }
}

module queue_msg './role.bicep' = {
  name: 'queue_msg-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '8a0f0c08-91a1-4084-bc3d-661d67233fed'
    resourceGroupName: resourceGroupName
  }
}

module sb_receiver './role.bicep' = {
  name: 'sb_receiver-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
    resourceGroupName: resourceGroupName
  }
}

module sb_sender './role.bicep' = {
  name: 'sb_sender-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
    resourceGroupName: resourceGroupName
  }
}

module cog_user './role.bicep' = {
  name: 'cog_user-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: 'a97b65f3-24c7-4388-baec-2e87135dc908'
    resourceGroupName: resourceGroupName
  }
}

module app_config './role.bicep' = {
  name: 'app_config-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b'
    resourceGroupName: resourceGroupName
  }
}

module acr_perms './rolesacr.bicep' = {
  name: 'acr_perms-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    resourceGroupName: resourceGroupName
  }
}