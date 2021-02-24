using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Cosmos;
using Azure.Cosmos.Serialization;
using Azure.Security.KeyVault.Secrets;
using Lib.Model;

namespace Lib.Data.Providers
{
    public class CosmosDataProvider : IDataProvider, IDisposable
    {
        public CosmosClient CosmosClient;
        public CosmosContainer CosmosContainer;
        public SecretClient SecretClient;

        public async Task InitializeAsync(TokenCredential credential)
        {
            // KeyVault
            SecretClient = new SecretClient(Config.KeyVaultEndpoint, credential);
            var cosmosKey = await SecretClient.GetSecretAsync(Config.CosmosKeySecretName);

            CosmosClientOptions options = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                ConsistencyLevel = ConsistencyLevel.Session,
                Diagnostics = {
                    IsLoggingEnabled = false
                },
                SerializerOptions = new CosmosSerializationOptions { PropertyNamingPolicy = CosmosPropertyNamingPolicy.Default }
            };

            // Cosmos
            CosmosClient = new CosmosClient(
                Config.CosmosEndpoint,
                cosmosKey.Value.Value,
                options);

            CosmosContainer = CosmosClient.GetDatabase(Config.CosmosDB).GetContainer(Config.CosmosCollection);
        }

        public void Dispose()
        {
            if (CosmosClient != null)
                CosmosClient.Dispose();
        }

        public async Task<Image> GetImageAsync(string id)
        {
            QueryDefinition queryDefinition = new QueryDefinition("SELECT * FROM c F WHERE F.id = @id").WithParameter("@id", id);

            await foreach (Image item in CosmosContainer.GetItemQueryIterator<Image>(queryDefinition))
            {
                return item;
            }
            return null;
        }

        public async Task<Image> DeleteImageAsync(string id)
        {
            var cosmosImage = new CosmosImage { Id = id };
            var response = await CosmosContainer.DeleteItemAsync<Image>(id, cosmosImage.PartitionKeyValue);
            return response.Value;

            //TODO : Bubble up errors through the stack
        }

        public async IAsyncEnumerable<Image> GetImagesAsync()
        {
            await foreach (var item in CosmosContainer.GetItemQueryIterator<Image>("SELECT * FROM c F ORDER BY F.createdDate DESC"))
            {
                yield return item;
            }
        }

        public async Task<Image> UpsertImageAsync(IImage image)
        {
            var response = await CosmosContainer.UpsertItemAsync(image as Image);
            return response.Value;
        }

        public IImage DeserializeImage(string json)
        {
            return JsonSerializer.Deserialize<Image>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }
    }
}