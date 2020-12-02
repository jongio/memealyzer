param basename string = ''
param location string = 'westus2'
param failover_location string = 'eastus2'
param cli_user_id string = ''

resource storage 'Microsoft.Storage/storageAccounts@2019-06-01' = {
  name: '${basename}storage'
  location: location
  kind: 'StorageV2'
  sku: {
    name: 'Standard_LRS'
  }
  properties: {}
}

resource form_recognizer 'Microsoft.CognitiveServices/accounts@2017-04-18' = {
  name: '${basename}fr'
  location: location
  kind: 'FormRecognizer'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: '${basename}fr'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource text_analytics 'Microsoft.CognitiveServices/accounts@2017-04-18' = {
  name: '${basename}ta'
  location: location
  kind: 'TextAnalytics'
  sku: {
    name: 'S0'
  }
  properties: {
    customSubDomainName: '${basename}ta'
  }
  identity: {
    type: 'SystemAssigned'
  }
}

resource key_vault 'Microsoft.KeyVault/vaults@2019-09-01' = {
  name: '${basename}kv'
  location: location
  properties: {
    tenantId: subscription().tenantId
    sku: {
      family: 'A'
      name: 'standard'
    }
    accessPolicies: [
      {
        objectId: cli_user_id
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
      {
        
        objectId: aks.properties.identityProfile.kubeletidentity.objectId
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
      {
        objectId: aks.identity.principalId
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
      // Cannot have the below because of circular reference with function/kv
      // {
      //   objectId: function.identity.principalId
      //   permissions: {
      //     secrets: [
      //       'get'
      //       'set'
      //       'list'
      //       'delete'
      //     ]
      //   }
      //   tenantId: subscription().tenantId
      // }
    ]
  }
}

resource cosmos_key_secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${key_vault.name}/CosmosKey'
  properties: {
    value: listKeys(cosmos_account.id, cosmos_account.apiVersion).primaryMasterKey
  }
}

resource storage_key_secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${key_vault.name}/StorageKey'
  properties: {
    value: listKeys(storage.id, storage.apiVersion).keys[0].value
  }
}

resource signalr_connection_string_secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${key_vault.name}/SignalRConnectionString'
  properties: {
    value: listKeys(signalr.id, signalr.apiVersion).primaryConnectionString
  }
}

resource storage_connection_string_secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${key_vault.name}/StorageConnectionString'
  properties: {
    value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storage.id, storage.apiVersion).keys[0].value}'
  }
}

resource service_bus_connection_string_secret 'Microsoft.KeyVault/vaults/secrets@2019-09-01' = {
  name: '${key_vault.name}/ServiceBusConnectionString'
  properties: {
    value: listKeys(resourceId('Microsoft.ServiceBus/namespaces/authorizationRules', service_bus.name, 'RootManageSharedAccessKey'), service_bus.apiVersion).primaryConnectionString
  }
}

resource cosmos_account 'Microsoft.DocumentDB/databaseAccounts@2020-04-01' = {
  name: '${basename}cosmosaccount'
  location: location
  kind: 'GlobalDocumentDB'
  properties: {
    databaseAccountOfferType: 'Standard'
    enableAutomaticFailover: false
    consistencyPolicy: {
      defaultConsistencyLevel: 'Session'
    }
    locations: [
      {
        failoverPriority: 1
        locationName: failover_location
      }
      {
        failoverPriority: 0
        locationName: location
      }
    ]
  }
}

resource cosmos_sqldb 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases@2020-04-01' = {
  name: '${cosmos_account.name}/memealyzer'
  properties: {
    options: {
      throughput: 400
    }
    resource: {
      id: 'memealyzer'
    }
  }
}

resource cosmos_sqldb_container 'Microsoft.DocumentDB/databaseAccounts/sqlDatabases/containers@2020-04-01' = {
  name: '${cosmos_account.name}/memealyzer/images'
  properties: {
    options: {
      throughput: 400
    }
    resource: {
      partitionKey: {
        paths: [
          '/uid'
        ]
      }
      id: 'images'
      uniqueKeyPolicy: {
        uniqueKeys: [
          {
            paths: [
              '/uid'
            ]
          }
        ]
      }
    }
  }
}

resource aks 'Microsoft.ContainerService/managedClusters@2020-09-01' = {
  name: '${basename}aks'
  location: location
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    kubernetesVersion: '1.19.0'
    nodeResourceGroup: '${basename}aksnodes'
    dnsPrefix: '${basename}aks'

    agentPoolProfiles: [
      {
        name: 'default'
        count: 1
        vmSize: 'Standard_A2_v2'
        mode: 'System'
      }
    ]
  }
}

resource logging 'Microsoft.Insights/components@2015-05-01' = {
  name: '${basename}ai'
  location: location
  kind: 'web'
  properties: {
    Application_Type: 'web'
  }
}

resource appconfig 'Microsoft.AppConfiguration/configurationStores@2020-06-01' = {
  name: '${basename}appconfig'
  location: location
  sku: {
    name: 'Standard'
  }
  identity: {
    type: 'SystemAssigned'
  }
  properties: {}
}

resource appconfig_borderstyle 'Microsoft.AppConfiguration/configurationStores/keyValues@2020-07-01-preview' = {
  name: '${appconfig.name}/borderStyle'
  properties: {
    value: 'solid'
  }
}

resource signalr 'Microsoft.SignalRService/signalR@2020-07-01-preview' = {
  name: '${basename}signalr'
  location: location

  sku: {
    name: 'Standard_S1'
    capacity: 1
  }

  properties: {
    cors: {
      allowedOrigins: [
        '*'
      ]
    }
    features: [
      {
        flag: 'ServiceMode'
        value: 'Serverless'
      }
    ]
  }
}

resource plan 'Microsoft.Web/serverfarms@2020-06-01' = {
  name: '${basename}plan'
  location: location
  sku: {
    tier: 'Standard'
    size: 'S1'
    name: 'S1'
  }
  properties: {}
}

resource function 'Microsoft.Web/sites@2020-06-01' = {
  name: '${basename}function'
  location: location
  kind: 'functionapp'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    siteConfig: {
      alwaysOn: true
      cors: {
        allowedOrigins: [
          '*'
        ]
        supportCredentials: false
      }
      appSettings: [
        {
          name: 'AzureWebJobsStorage'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storage.id, storage.apiVersion).keys[0].value}'
        }
        {
          name: 'WEBSITE_CONTENTAZUREFILECONNECTIONSTRING'
          value: 'DefaultEndpointsProtocol=https;AccountName=${storage.name};EndpointSuffix=${environment().suffixes.storage};AccountKey=${listKeys(storage.id, storage.apiVersion).keys[0].value}'
        }
        {
          name: 'APPINSIGHTS_INSTRUMENTATIONKEY'
          value: logging.properties.InstrumentationKey
        }
        {
          name: 'APPLICATIONINSIGHTS_CONNECTION_STRING'
          value: 'InstrumentationKey=${logging.properties.InstrumentationKey}'
        }
        {
          name: 'FUNCTIONS_WORKER_RUNTIME'
          value: 'dotnet'
        }
        {
          name: 'FUNCTIONS_EXTENSION_VERSION'
          value: '~3'
        }
        {
          name: 'WEBSITES_ENABLE_APP_SERVICE_STORAGE'
          value: 'false'
        }
        {
          name: 'AZURE_KEYVAULT_ENDPOINT'
          value: key_vault.properties.vaultUri
        }
        {
          name: 'AZURE_CLIENT_SYNC_QUEUE_NAME'
          value: 'sync'
        }
        {
          name: 'AZURE_STORAGE_CONNECTION_STRING_SECRET_NAME'
          value: 'StorageConnectionString'
        }
        {
          name: 'AZURE_SIGNALR_CONNECTION_STRING_SECRET_NAME'
          value: 'SignalRConnectionString'
        }
        {
          name: 'WEBSITE_RUN_FROM_PACKAGE'
          value: ''
        }
      ]
    }
  }
}

resource acr 'Microsoft.ContainerRegistry/registries@2019-12-01-preview' = {
  name: '${basename}acr'
  location: location
  sku: {
    name: 'Standard'
  }
  properties: {
    adminUserEnabled: true
  }
}

resource service_bus 'Microsoft.ServiceBus/namespaces@2017-04-01' = {
  name: '${basename}sb'
  location: location
  sku: {
    name: 'Basic'
  }
}

resource messages 'Microsoft.ServiceBus/namespaces/queues@2017-04-01' = {
  name: '${service_bus.name}/messages'
  properties: {
    defaultMessageTimeToLive: 'PT30S'
  }
}

resource sync 'Microsoft.ServiceBus/namespaces/queues@2017-04-01' = {
  name: '${service_bus.name}/sync'
  properties: {
    defaultMessageTimeToLive: 'PT30S'
  }
}

// Not using due to permission update not allowed issue, resorting to perms.sh script
// module cli_perms './perms.bicep' = {
//   name: 'cli_perms'
//   params: {
//     principalId: cli_user_id
//     principalType: 'User'
//   }
// }

// module aks_cluster_perms './perms.bicep' = {
//   name: 'aks_cluster_perms'
//   params: {
//     principalId: aks.identity.principalId
//   }
// }

// module aks_node_pool_perms './perms.bicep' = {
//   name: 'aks_node_pool_perms'
//   params: {
//     principalId: reference(resourceId('${basename}aksnodes', 'Microsoft.ManagedIdentity/userAssignedIdentities', '${basename}aks-agentpool'), '2018-11-30').principalId
//   }
// }

// module function_perms './perms.bicep' = {
//   name: 'function_perms'
//   params: {
//     principalId: function.identity.principalId
//   }
// }