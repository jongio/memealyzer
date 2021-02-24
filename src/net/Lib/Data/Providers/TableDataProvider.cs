using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Data.Tables;
using Azure.Security.KeyVault.Secrets;
using Lib.Model;

namespace Lib.Data.Providers
{
    public class TableDataProvider : IDataProvider, IDisposable
    {
        public TableServiceClient TableServiceClient;
        public TableClient TableClient;
        public SecretClient SecretClient;

        public async Task InitializeAsync(TokenCredential credential)
        {
            var storageKeyValue = string.Empty;

            if (Config.StorageTableEndpoint.ToString().Contains("127"))
            {
                storageKeyValue = Config.AzuriteAccountKey;
            }
            else
            {
                // KeyVault
                SecretClient = new SecretClient(Config.KeyVaultEndpoint, credential);
                var storageKey = await SecretClient.GetSecretAsync(Config.StorageKeySecretName);
                storageKeyValue = storageKey.Value.Value;
            }

            TableClient = new TableClient(Config.StorageTableEndpoint,
                Config.StorageTableName,
                new TableSharedKeyCredential(Config.StorageAccountName, storageKeyValue));

            await TableClient.CreateIfNotExistsAsync();
        }

        public void Dispose()
        {
        }

        public async Task<Image> GetImageAsync(string id)
        {
            var image = new TableImage() { Id = id };

            await foreach (TableImage item in TableClient.QueryAsync<TableImage>(i => i.PartitionKey == image.PartitionKey && i.RowKey == image.RowKey))
            {
                return item;
            }
            return null;
        }

        public async Task<Image> DeleteImageAsync(string id)
        {
            var image = new TableImage { Id = id };
            await TableClient.DeleteEntityAsync(image.PartitionKey, image.RowKey);
            return image;

            //TODO : Bubble up errors through the stack
        }

        public async IAsyncEnumerable<Image> GetImagesAsync()
        {
            await foreach (var item in TableClient.QueryAsync<TableImage>())
            {
                yield return item;
            }
        }

        public async Task<Image> UpsertImageAsync(IImage image)
        {
            var response = await TableClient.UpsertEntityAsync<TableImage>(image as TableImage);
            return image as TableImage;
        }

        public IImage DeserializeImage(string json)
        {
            return JsonSerializer.Deserialize<TableImage>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}