param principalId string
param principalType string = 'ServicePrincipal'
param resourceGroupName string

var roles = [
  {
    name: 'blob_contrib-${principalId}'
    id: 'ba92f5b4-2d11-453d-a403-e96b0029c9fe'
  }
  {
    name: 'queue_contrib-${principalId}'
    id: '974c5e8b-45b9-4653-ba55-5f855dd0fb88'
  }
  {
    name: 'queue_msg-${principalId}'
    id: '8a0f0c08-91a1-4084-bc3d-661d67233fed'
  }
  {
    name: 'sb_receiver-${principalId}'
    id: '4f6d3b9b-027b-4f4c-9142-0e5a2a2247e0'
  }
  {
    name: 'sb_sender-${principalId}'
    id: '69a216fc-b8fb-44d8-bc22-1f3c2cd27a39'
  }
  {
    name: 'cog_user-${principalId}'
    id: 'a97b65f3-24c7-4388-baec-2e87135dc908'
  }
  {
    name: 'app_config-${principalId}'
    id: '5ae67dd6-50cb-40e7-96ff-dc2bfa4b606b'
  }
]

module appRoleAssignments './roles.bicep' = {
  name: 'app_role_assignments-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    resourceGroupName: resourceGroupName
    roles: roles
  }
}

module acrRoleAssignments './rolesacr.bicep' = {
  name: 'acr_role_assignments-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    resourceGroupName: resourceGroupName
  }
}