using System;
using System.Linq;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch;
using Microsoft.Azure.CognitiveServices.Search.ImageSearch.Models;
using System.Threading.Tasks;
using Lib.Model;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Net.Http;

namespace Lib.Images.Providers
{
    public class BingImageSearchProvider : IImageProvider
    {
        const string uriBase = "https://api.bing.microsoft.com/v7.0/images/search";

        struct SearchResult
        {
            public string jsonResult;
            public Dictionary<String, String> relevantHeaders;
        }

        public async Task<Image> GetImage()
        {
            var images = new List<Image>();
            SearchResult result = await BingImageSearch(Clients.SearchTerm);
            dynamic jsonObj = Newtonsoft.Json.JsonConvert.DeserializeObject(result.jsonResult);

            // pick a random one from the list
            var len = ((JArray)jsonObj["value"]).Count;
            var indx = new Random().Next(0, len - 1);
            var firstJsonObj = jsonObj["value"][indx];

            return new Image { Url = firstJsonObj["contentUrl"] };
        }

        static async Task<SearchResult> BingImageSearch(string searchQuery)
        {
            var uriQuery = uriBase + "?count=100&q=" + Uri.EscapeDataString(searchQuery);
            
            HttpRequestMessage request = new HttpRequestMessage() 
            {
                RequestUri = new Uri(uriQuery),
                Method = HttpMethod.Get
            };

            request.Headers.Add("Ocp-Apim-Subscription-Key", Config.BingSearchKey);
            var httpClient = new HttpClient();
            var response = await httpClient.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            
            var searchResult = new SearchResult()
            {
                jsonResult = json,
                relevantHeaders = new Dictionary<String, String>()
            };

            return searchResult;
        }
    }
}