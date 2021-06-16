using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Lib.Model;
using Memealyzer;

namespace Lib.Media
{
    public class ImageClient : IImageClient
    {
        private HttpClient httpClient;

        public ImageClient(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        public async Task<Image> GetRandomImage()
        {
            return await httpClient.GetFromJsonAsync<Image>(Config.ImageEndpoint);
        }

        public async Task<Stream> GetImageStream(Image image)
        {
            if (image?.Url is null || string.IsNullOrEmpty(image.Url))
            {
                var tempImage = await GetRandomImage();
                tempImage.Id = image.Id;
                image = tempImage;
            }

            return await httpClient.GetStreamAsync(image.Url);
        }

        public async Task<Stream> GetRandomImageStream(string url)
        {
            var image = await GetRandomImage();
            return await GetImageStream(image);
        }
    }
}