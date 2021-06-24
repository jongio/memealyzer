using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Data.Tables;
using Lib.Configuration;
using Lib.Model;

namespace Lib.Data.Providers
{
    public class TableDataProvider : IDataProvider, IDisposable
    {
        private TableClient tableClient;

        public async Task InitializeAsync(TokenCredential credential)
        {
            tableClient = Config.UseAzuriteTable ?
                new TableClient(Config.AzuriteConnectionString, Config.StorageTableName) :
                new TableClient(Config.StorageTableEndpoint, Config.StorageTableName, credential);

            await tableClient.CreateIfNotExistsAsync();
        }

        public void Dispose()
        {
        }

        public async Task<Image> GetImageAsync(string id)
        {
            var image = new TableImage() { Id = id };

            await foreach (TableImage item in tableClient.QueryAsync<TableImage>(i => i.PartitionKey == image.PartitionKey && i.RowKey == image.RowKey))
            {
                return item;
            }
            return null;
        }

        public async Task<Image> DeleteImageAsync(string id)
        {
            var image = new TableImage { Id = id };
            await tableClient.DeleteEntityAsync(image.PartitionKey, image.RowKey);
            return image;

            //TODO : Bubble up errors through the stack
        }

        public async IAsyncEnumerable<Image> GetImagesAsync()
        {
            await foreach (var item in tableClient.QueryAsync<TableImage>())
            {
                yield return item;
            }
        }

        public async Task<Image> UpsertImageAsync(IImage image)
        {
            var response = await tableClient.UpsertEntityAsync<TableImage>(image as TableImage);
            return image as TableImage;
        }

        public IImage DeserializeImage(string json)
        {
            return JsonSerializer.Deserialize<TableImage>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}