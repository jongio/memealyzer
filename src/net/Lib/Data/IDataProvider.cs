using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using Lib.Model;

namespace Lib.Data
{
    public interface IDataProvider : IDisposable
    {
        Task InitializeAsync(TokenCredential credential, SecretClient secretClient);
        IAsyncEnumerable<Image> GetImagesAsync();
        Task<Image> GetImageAsync(string id);
        Task<Image> DeleteImageAsync(string id);
        Task<Image> UpsertImageAsync(IImage image);
        IImage DeserializeImage(string json);
    }
}