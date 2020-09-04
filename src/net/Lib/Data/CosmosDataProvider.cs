using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Cosmos;
using Azure.Security.KeyVault.Secrets;
using DotNetEnv;

namespace Lib.Data
{
    public class CosmosDataProvider : IDataProvider
    {
        public CosmosClient CosmosClient;
        public CosmosContainer CosmosContainer;
        public SecretClient SecretClient;

        public CosmosDataProvider()
        {
        }

        public IImage DeserializeImage(string json)
        {
            return JsonSerializer.Deserialize<Image>(json, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        }

        public async Task InitializeAsync(TokenCredential credential)
        {
            // KeyVault
            SecretClient = new SecretClient(new Uri(Env.GetString("AZURE_KEYVAULT_ENDPOINT")), credential);
            var cosmosKey = await SecretClient.GetSecretAsync(Env.GetString("AZURE_COSMOS_KEY_SECRET_NAME", "cosmoskey"));

            // Cosmos
            CosmosClient = new CosmosClient(
                Env.GetString("AZURE_COSMOS_ENDPOINT"),
                cosmosKey.Value.Value,
                new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Direct,
                    ConsistencyLevel = ConsistencyLevel.Session
                });

            CosmosContainer = CosmosClient.GetDatabase(Env.GetString("AZURE_COSMOS_DB")).GetContainer(Env.GetString("AZURE_COSMOS_COLLECTION"));
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
    }
}