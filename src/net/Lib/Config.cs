using DotNetEnv;
using System;

namespace Lib
{
    public static class Config
    {
        public static readonly string BaseName = Env.GetString("BASENAME");
        public static readonly string CosmosEndpoint = String.Format(Env.GetString("AZURE_COSMOS_ENDPOINT", "https://{0}cosmosaccount.documents.azure.com:443/"), BaseName);
        public static readonly string CosmosDb = Env.GetString("AZURE_COSMOS_DB", "memealyzer");
        public static readonly string CosmosCollection = Env.GetString("AZURE_COSMOS_COLLECTION", "images");
        public static readonly string CosmosKeySecretName = Env.GetString("AZURE_STORAGE_KEY_SECRET_NAME", "StorageKey");
        public static readonly string StorageType = Env.GetString("AZURE_STORAGE_TYPE", "COSMOS_SQL");
        public static readonly string FormRecognizerEndpoint = String.Format(Env.GetString("AZURE_FORM_RECOGNIZER_ENDPOINT", "https://{0}formrecognizer.cognitiveservices.azure.com/"), BaseName);
        public static readonly string KeyVaultEndpoint = String.Format(Env.GetString("AZURE_KEYVAULT_ENDPOINT", "https://{0}kv.vault.azure.net/"), BaseName);
        public static readonly string StorageAccountName = String.Format(Env.GetString("AZURE_STORAGE_ACCOUNT_NAME", "{0}storage"), BaseName);
        public static readonly string StorageBlobEndpoint = String.Format(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT", "https://{0}storage.blob.core.windows.net/"), BaseName);
        public static readonly string StorageQueueEndpoint = String.Format(Env.GetString("AZURE_STORAGE_QUEUE_ENDPOINT", "https://{0}storage.queue.core.windows.net/"), BaseName);
        public static readonly string StorageTableEndpoint = String.Format(Env.GetString("AZURE_STORAGE_TABLE_ENDPOINT", "https://{0}storage.table.core.windows.net/"), BaseName);
        public static readonly string TextAnalyticsEndpoint = String.Format(Env.GetString("AZURE_TEXT_ANALYTICS_ENDPOINT", "https://{0}textanalytics.cognitiveservices.azure.com/"), BaseName);
        public static readonly string AppConfigEndpoint = String.Format(Env.GetString("AZURE_APP_CONFIG_ENDPOINT", "https://{0}appconfig.azconfig.io"), BaseName);
        public static readonly string ContainerRegistryServer = String.Format(Env.GetString("AZURE_CONTAINER_REGISTRY_SERVER", "{0}acr.azurecr.io"), BaseName);



    }
}