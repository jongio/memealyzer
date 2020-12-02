// Not using due to permission update not allowed issue, resorting to perms.sh script

param principalId string = ''
param principalType string = 'ServicePrincipal'

module acr_push './role.bicep' = {
  name: 'acr_push'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '8311e382-0749-4cb8-b61a-304f252e45ec'
  }
}

module acr_pull './role.bicep' = {
  name: 'acr_pull'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
  }
}