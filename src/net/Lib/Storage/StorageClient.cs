using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Lib.Model;

namespace Lib.Storage
{
    public class StorageClient : IStorageClient
    {
        private BlobContainerClient ContainerClient;

        public async Task InitializeAsync(TokenCredential credential)
        {
            var blobServiceClient = new BlobServiceClient(Config.StorageBlobEndpoint, credential);
            ContainerClient = blobServiceClient.GetBlobContainerClient(Config.StorageBlobContainerName);
            await ContainerClient.CreateIfNotExistsAsync(PublicAccessType.BlobContainer);
        }

        public async Task<BlobInfo> UploadBlob(string name, Stream stream)
        {
            var blobClient = ContainerClient.GetBlobClient(name);
            var blobContentInfo = await blobClient.UploadAsync(stream);
            return new BlobInfo { BlobContentInfo = blobContentInfo, Uri = blobClient.Uri };
        }
    }
}