param basename string
param principalId string

resource kv 'Microsoft.KeyVault/vaults@2019-09-01' existing = {
  name: '${basename}kv'

  resource kv_ap 'accessPolicies' = {
    name: 'add'
    properties: {
      accessPolicies: [
        {
          objectId: principalId
          permissions: {
            secrets: [
              'get'
              'set'
              'list'
              'delete'
            ]
          }
          tenantId: subscription().tenantId
        }
      ]
    }
  }
}
