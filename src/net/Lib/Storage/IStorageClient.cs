using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs.Models;
using Lib.Model;

namespace Lib.Storage
{
    public interface IStorageClient
    {
        Task InitializeAsync(TokenCredential credential);
        Task<BlobInfo> UploadBlobAsync(Image name, Stream stream);
    }
}