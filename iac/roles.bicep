param principalId string
param principalType string = 'ServicePrincipal'
param resourceGroupName string
param roles array

module roleAssignments './role.bicep' = [for role in roles: {
  name: role.name
  params: {
    principalId: principalId
    principalType: principalType
    roleId: role.id
    resourceGroupName: resourceGroupName
  }
}]