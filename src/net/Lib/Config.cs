using System;
using DotNetEnv;

namespace Lib
{
    public static class Config
    {
        public static string BaseName { get { return Env.GetString("BASENAME"); } }
        public static bool UseAzurite { get { return Env.GetBool("USE_AZURITE", false); } }
        public static string CosmosEndpoint { get { return String.Format(Env.GetString("AZURE_COSMOS_ENDPOINT", "https://{0}cosmosaccount.documents.azure.com:443"), BaseName); } }
        public static Uri FormRecognizerEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_FORM_RECOGNIZER_ENDPOINT", "https://{0}fr.cognitiveservices.azure.com/"), BaseName)); } }
        public static Uri KeyVaultEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_KEYVAULT_ENDPOINT", "https://{0}kv.vault.azure.net/"), BaseName)); } }
        public static string StorageAccountName { get { return String.Format(Env.GetString("AZURE_STORAGE_ACCOUNT_NAME", UseAzurite ? "devstoreaccount1" : "{0}storage"), BaseName); } }
        public static Uri StorageBlobEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT", UseAzurite ? "https://127.0.0.1:10000/devstoreaccount1" : "https://{0}storage.blob.core.windows.net/"), BaseName)); } }
        public static Uri StorageQueueEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT", UseAzurite ? "https://127.0.0.1:10001/devstoreaccount1" : "https://{0}storage.queue.core.windows.net/"), BaseName)); } }
        public static Uri StorageTableEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_STORAGE_TABLE_ENDPOINT", UseAzurite ? "http://127.0.0.1:10002/devstoreaccount1" : "https://{0}storage.table.core.windows.net/"), BaseName)); } }
        public static Uri TextAnalyticsEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_TEXT_ANALYTICS_ENDPOINT", "https://{0}ta.cognitiveservices.azure.com/"), BaseName)); } }
        public static Uri AppConfigEndpoint { get { return new Uri(String.Format(Env.GetString("AZURE_APP_CONFIG_ENDPOINT", "https://{0}appconfig.azconfig.io"), BaseName)); } }
        public static string ServiceBusNamespace { get { return String.Format(Env.GetString("AZURE_SERVICE_BUS_NAMESPACE", "{0}sb.servicebus.windows.net"), BaseName); } }
        public static string StorageBlobContainerName { get { return Env.GetString("AZURE_STORAGE_BLOB_CONTAINER_NAME", "blobs"); } }
        public static string MessagesQueueName { get { return Env.GetString("AZURE_MESSAGES_QUEUE_NAME", "messages"); } }
        public static int StorageQueueMessageCount { get { return Env.GetInt("AZURE_STORAGE_QUEUE_MSG_COUNT", 10); } }
        public static int StorageQueueReceiveSleep { get { return Env.GetInt("AZURE_STORAGE_QUEUE_RECEIVE_SLEEP", 1); } }
        public static string StorageTableName { get { return Env.GetString("AZURE_STORAGE_TABLE_NAME", "images"); } }
        public static string CosmosDB { get { return Env.GetString("AZURE_COSMOS_DB", "memealyzer"); } }
        public static string CosmosCollection { get { return Env.GetString("AZURE_COSMOS_COLLECTION", "images"); } }
        public static string CosmosKeySecretName { get { return Env.GetString("AZURE_COSMOS_KEY_SECRET_NAME", "CosmosKey"); } }
        public static string StorageType { get { return Env.GetString("AZURE_STORAGE_TYPE", "COSMOS_SQL"); } }
        public static string MessagingType { get { return Env.GetString("AZURE_MESSAGING_TYPE", "SERVICE_BUS_QUEUE"); } }
        public static string AzuriteAccountKeySecretName { get { return Env.GetString("AZURITE_ACCOUNT_KEY_SECRET_NAME", "AzuriteKey"); } }
        public static string StorageKeySecretName { get { return UseAzurite ? AzuriteAccountKeySecretName : Env.GetString("AZURE_STORAGE_KEY_SECRET_NAME", "StorageKey"); } }
        public static string ClientSyncQueueName { get { return Env.GetString("AZURE_CLIENT_SYNC_QUEUE_NAME", "sync"); } }
        public static string SignalRConnectionStringSecretName { get { return Env.GetString("AZURE_SIGNALR_CONNECTION_STRING_SECRET_NAME", "SignalRConnectionString"); } }
        public static string StorageConnectionStringSecretName { get { return Env.GetString("AZURE_STORAGE_CONNECTION_STRING_SECRET_NAME", "StorageConnectionString"); } }
        public static string ServiceBusConnectionStringSecretName { get { return Env.GetString("AZURE_SERVICE_BUS_CONNECTION_STRING_SECRET_NAME", "ServiceBusConnectionString"); } }
        public static string MemeEndpoint { get { return Env.GetString("MEME_ENDPOINT", "https://meme-api.herokuapp.com/gimme/wholesomememes"); } }
        public static string CosmosKey { get { return Env.GetString("AZURE_COSMOS_KEY"); } }
        public static string AzuriteAccountKey { get { return Env.GetString("AZURITE_ACCOUNT_KEY"); } }
    }
}