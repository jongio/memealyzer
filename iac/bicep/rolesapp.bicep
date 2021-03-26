param principalId string
param principalType string = 'ServicePrincipal'
param resourceGroupName string



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
