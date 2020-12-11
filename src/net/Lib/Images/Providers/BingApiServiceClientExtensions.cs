using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Lib.Images.Providers
{
    public static class BingHttpClientExtensions
    {
        public static async Task<BingImageProviderConfiguration> GetSearchTerm(this HttpClient client)
        {
            try
            {
                var term = await client.GetFromJsonAsync<BingImageProviderConfiguration>("/config/bing");
                return term;
            }
            catch { }
            return null;
        }
    }
}