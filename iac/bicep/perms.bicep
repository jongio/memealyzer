// Not using due to permission update not allowed issue, resorting to perms.sh script

param principalId string = ''
param principalType string = 'ServicePrincipal'

module blob_contrib './role.bicep' = {
  name: 'blob_contrib'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  }
}

module queue_contrib './role.bicep' = {
  name: 'queue_contrib'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
  }
}

module queue_msg './role.bicep' = {
  name: 'queue_msg'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '8a0f0c08-91a1-4084-bc3d-661d67233fed'
  }
}

module sb_receiver './role.bicep' = {
  name: 'sb_receiver'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
  }
}

module sb_sender './role.bicep' = {
  name: 'sb_sender'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
  }
}

module cog_user './role.bicep' = {
  name: 'cog_user'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: 'a97b65f3-24c7-4388-baec-2e87135dc908'
  }
}

module app_config './role.bicep' = {
  name: 'app_config'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b'
  }
}

module acr_perms './permsacr.bicep' = {
  name: 'acr_perms'
  params: {
    principalId: principalId
    principalType: principalType
  }
}