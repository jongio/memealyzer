using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lib.Configuration;
using Lib.Model;

namespace Lib.Storage
{
    public class StorageClient : IStorageClient
    {
        private BlobContainerClient ContainerClient;

        public async Task InitializeAsync(TokenCredential credential)
        {
            var blobServiceClient = Config.UseAzuriteBlob ?
                new BlobServiceClient(Config.AzuriteConnectionString) :
                new BlobServiceClient(Config.StorageBlobEndpoint, credential);

            ContainerClient = blobServiceClient.GetBlobContainerClient(Config.StorageBlobContainerName);
            await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task<BlobInfo> UploadBlobAsync(Image image, Stream stream)
        {
            var blobClient = ContainerClient.GetBlobClient(image.BlobName);
            var blobContentInfo = await blobClient.UploadAsync(stream);
            image.BlobUri = blobClient.Uri.ToString();
            
            return new BlobInfo { BlobContentInfo = blobContentInfo, Image = image };
        }
    }
}