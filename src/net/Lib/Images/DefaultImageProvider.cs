using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Lib.Model;

namespace Lib.Images
{
    public class DefaultImageSource : IImageProvider
    {
        private HttpClient httpClient = new HttpClient();
        
        public async Task<Image> GetImage()
        {
            var memeImage = await httpClient.GetFromJsonAsync<Image>(Config.MemeEndpoint);
            return memeImage;
        }
    }
}
