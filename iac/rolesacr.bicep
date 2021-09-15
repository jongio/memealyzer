param principalId string
param principalType string = 'ServicePrincipal'
param resourceGroupName string

var roles = [
  {
    name: 'acr_push-${principalId}'
    id: '8311e382-0749-4cb8-b61a-304f252e45ec'
  }
  {
    name: 'acr_pull-${principalId}'
    id: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  }
]

module roleAssignments './roles.bicep' = {
  name: 'acr_roles-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    resourceGroupName: resourceGroupName
    roles: roles
  }
}