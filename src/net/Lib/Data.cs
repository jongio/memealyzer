using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Search.Documents;
using Azure.Search.Documents.Models;
using Azure.Search.Documents.Indexes;
using DotNetEnv;

namespace Lib
{
    public class Data
    {
        public BlobServiceClient BlobServiceClient;
        public BlobContainerClient ContainerClient;
        public SearchClient SearchClient;
        public SearchIndexerClient SearchIndexerClient;

        private TokenCredential credential = Identity.GetCredentialChain();

        public Data()
        {
        }

        public async Task InitializeAsync()
        {
            // Blob
            BlobServiceClient = new BlobServiceClient(new Uri(Env.GetString("AZURE_STORAGE_BLOB_ENDPOINT")), credential);
            ContainerClient = BlobServiceClient.GetBlobContainerClient(Env.GetString("AZURE_STORAGE_BLOB_CONTAINER_NAME"));
            await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);

            // Search
            var keyCredential = new Azure.AzureKeyCredential(Env.GetString("AZURE_SEARCH_KEY"));
            SearchClient = new SearchClient(
                new Uri(Env.GetString("AZURE_SEARCH_ENDPOINT")),
                Env.GetString("AZURE_SEARCH_INDEX_NAME"),
                keyCredential);

            SearchIndexerClient = new SearchIndexerClient(
                new Uri(Env.GetString("AZURE_SEARCH_ENDPOINT")),
                keyCredential);
        }

        public async Task<Image> EnqueueImageAsync(Image image = null)
        {
            using var http = new HttpClient();

            if (image?.Url is null || string.IsNullOrEmpty(image.Url))
            {
                image = await http.GetFromJsonAsync<Image>(Env.GetString("MEME_ENDPOINT"));
            }

            // Get Image Stream
            using var imageStream = await http.GetStreamAsync(image.Url);

            // Upload to Blob
            var blobClient = ContainerClient.GetBlobClient(image.BlobName);

            Console.WriteLine($"Uploading {image.Url} to Blob Storage: {blobClient.Uri}");
            await blobClient.UploadAsync(imageStream);

            Console.WriteLine($"Uploaded to Blob Storage: {blobClient.Uri}");

            image.BlobUri = blobClient.Uri.ToString();

            // Re-run the indexer (in practice, you may do this batches)
            await SearchIndexerClient.RunIndexerAsync(Env.GetString("AZURE_SEARCH_INDEXER_NAME"));

            return image;
        }

        public async IAsyncEnumerable<Image> GetImagesAsync()
        {
            SearchResults<Image> images = await SearchClient.SearchAsync<Image>("*", new SearchOptions
            {
                OrderBy = { "createdDate desc" },
            });

            await foreach (SearchResult<Image> image in images.GetResultsAsync())
            {
                yield return image.Document;
            }
        }

        public async Task<Image> GetImageAsync(string id)
        {
            return await SearchClient.GetDocumentAsync<Image>(id);
        }
    }
}
