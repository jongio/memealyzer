// Not using due to permission update not allowed issue, resorting to perms.sh script

param basename string = ''

module acr_perms './permsacr.bicep' = {
  name: 'acr_perms'
  params: {
    principalId: reference(resourceId('${basename}aksnodes', 'Microsoft.ManagedIdentity/userAssignedIdentities', '${basename}aks-agentpool'), '2018-11-30').principalId
    principalType: 'ServicePrincipal'
  }
}