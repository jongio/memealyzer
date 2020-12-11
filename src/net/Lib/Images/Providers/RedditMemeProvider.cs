using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using Lib.Model;

namespace Lib.Images.Providers
{
    public class RedditMemeProvider : IImageProvider
    {
        private HttpClient httpClient = new HttpClient();
        
        public async Task<Image> GetImage() => 
            await httpClient.GetFromJsonAsync<Image>(Config.MemeEndpoint);
    }
}
