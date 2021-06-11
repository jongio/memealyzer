using System.IO;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Storage.Blobs.Models;

namespace Lib.Storage
{
    public interface IStorageClient
    {
        Task InitializeAsync(TokenCredential credential);
        Task<BlobInfo> UploadBlob(string name, Stream stream);
    }
}