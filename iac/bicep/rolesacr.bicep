param principalId string = ''
param principalType string = 'ServicePrincipal'
param rgName string = ''

module acr_push './role.bicep' = {
  name: 'acr_push-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '8311e382-0749-4cb8-b61a-304f252e45ec'
    rgName: rgName
  }
}

module acr_pull './role.bicep' = {
  name: 'acr_pull-${principalId}'
  params: {
    principalId: principalId
    principalType: principalType
    roleId: '7f951dda-4ed3-4680-a7ca-43fe172d538d'
    rgName: rgName
  }
}