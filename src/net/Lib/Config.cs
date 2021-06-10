using System;
using DotNetEnv;

namespace Lib
{
    public static class Config
    {
        public static string BaseName { get { return Env.GetString("BASENAME"); } }
        public static bool UseAzurite { get { return Env.GetBool("USE_AZURITE", false); } }
        public static bool UseCosmosEmulator { get { return Env.GetBool("USE_COSMOS_EMULATOR", false); } }

        public static string CosmosEndpoint
        {
            get
            {
                return Env.GetString("AZURE_COSMOS_ENDPOINT", UseCosmosEmulator ? "https://localhost:8081" : $"https://{BaseName}cosmosaccount.documents.azure.com:443");
            }
        }

        public static string CosmosKey
        {
            get
            {
                return Env.GetString("AZURE_COSMOS_KEY", UseCosmosEmulator ? "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==" : "");
            }

        }

        public static int CosmosThroughput { get { return Env.GetInt("AZURE_COSMOS_THROUGHPUT", 400); } }
        public static string CosmosDB { get { return Env.GetString("AZURE_COSMOS_DB", "memealyzer"); } }
        public static string CosmosCollection { get { return Env.GetString("AZURE_COSMOS_COLLECTION", "images"); } }
        public static string CosmosKeySecretName { get { return Env.GetString("AZURE_COSMOS_KEY_SECRET_NAME", "CosmosKey"); } }
        public static Uri FormRecognizerEndpoint { get { return new Uri(Env.GetString("AZURE_FORM_RECOGNIZER_ENDPOINT", $"https://{BaseName}fr.cognitiveservices.azure.com/")); } }
        public static Uri KeyVaultEndpoint { get { return new Uri(Env.GetString("AZURE_KEYVAULT_ENDPOINT", $"https://{BaseName}kv.vault.azure.net/")); } }
        public static string StorageAccountName { get { return Env.GetString("AZURE_STORAGE_ACCOUNT_NAME", UseAzurite ? "devstoreaccount1" : $"{BaseName}storage"); } }
        public static Uri StorageBlobEndpoint { get { return new Uri(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT", UseAzurite ? "https://127.0.0.1:10000/devstoreaccount1" : $"https://{BaseName}storage.blob.core.windows.net/")); } }
        public static Uri StorageQueueEndpoint { get { return new Uri(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT", UseAzurite ? "https://127.0.0.1:10001/devstoreaccount1" : $"https://{BaseName}storage.queue.core.windows.net/")); } }
        public static Uri StorageTableEndpoint { get { return new Uri(Env.GetString("AZURE_STORAGE_TABLE_ENDPOINT", UseAzurite ? "http://127.0.0.1:10002/devstoreaccount1" : $"https://{BaseName}storage.table.core.windows.net/")); } }
        public static Uri TextAnalyticsEndpoint { get { return new Uri(Env.GetString("AZURE_TEXT_ANALYTICS_ENDPOINT", $"https://{BaseName}ta.cognitiveservices.azure.com/")); } }
        public static Uri AppConfigEndpoint { get { return new Uri(Env.GetString("AZURE_APP_CONFIG_ENDPOINT", $"https://{BaseName}appconfig.azconfig.io")); } }
        public static string ServiceBusNamespace { get { return Env.GetString("AZURE_SERVICE_BUS_NAMESPACE", $"{BaseName}sb.servicebus.windows.net"); } }
        public static string StorageBlobContainerName { get { return Env.GetString("AZURE_STORAGE_BLOB_CONTAINER_NAME", "blobs"); } }
        public static string MessagesQueueName { get { return Env.GetString("AZURE_MESSAGES_QUEUE_NAME", "messages"); } }
        public static int StorageQueueMessageCount { get { return Env.GetInt("AZURE_STORAGE_QUEUE_MSG_COUNT", 10); } }
        public static int StorageQueueReceiveSleep { get { return Env.GetInt("AZURE_STORAGE_QUEUE_RECEIVE_SLEEP", 1); } }
        public static string StorageTableName { get { return Env.GetString("AZURE_STORAGE_TABLE_NAME", "images"); } }
        public static string StorageType { get { return Env.GetString("AZURE_STORAGE_TYPE", "COSMOS_SQL"); } }
        public static string MessagingType { get { return Env.GetString("AZURE_MESSAGING_TYPE", "SERVICE_BUS_QUEUE"); } }
        public static string AzuriteAccountKeySecretName { get { return Env.GetString("AZURITE_ACCOUNT_KEY_SECRET_NAME", "AzuriteKey"); } }
        public static string StorageKeySecretName { get { return UseAzurite ? AzuriteAccountKeySecretName : Env.GetString("AZURE_STORAGE_KEY_SECRET_NAME", "StorageKey"); } }
        public static string ClientSyncQueueName { get { return Env.GetString("AZURE_CLIENT_SYNC_QUEUE_NAME", "sync"); } }
        public static string SignalRConnectionStringSecretName { get { return Env.GetString("AZURE_SIGNALR_CONNECTION_STRING_SECRET_NAME", "SignalRConnectionString"); } }
        public static string MemeEndpoint { get { return Env.GetString("MEME_ENDPOINT", "https://meme-api.herokuapp.com/gimme/wholesomememes"); } }
        public static string AzuriteAccountKey { get { return Env.GetString("AZURITE_ACCOUNT_KEY", "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="); } }
    }
}