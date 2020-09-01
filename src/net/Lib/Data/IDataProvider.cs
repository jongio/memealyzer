using System.Collections.Generic;
using System.Threading.Tasks;
using Azure.Core;
using Lib;

namespace Lib.Data
{
    public interface IDataProvider
    {
        Task InitializeAsync(TokenCredential credential);
        IAsyncEnumerable<Image> GetImagesAsync();
        Task<Image> GetImageAsync(string id);
        Task<Image> UpsertImageAsync(IImage image);
        IImage DeserializeImage(string json);
    }
}