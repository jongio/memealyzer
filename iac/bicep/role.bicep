param principalId string
param principalType string = 'ServicePrincipal'
param resourceGroupName string
param roleId string

resource role_assignment 'Microsoft.Authorization/roleAssignments@2020-04-01-preview' = {
  name: guid(subscription().id, principalId, roleId, resourceGroupName)
  properties: {
    principalId: principalId
    principalType: principalType
    roleDefinitionId: resourceId('Microsoft.Authorization/roleDefinitions', roleId)
  }
}