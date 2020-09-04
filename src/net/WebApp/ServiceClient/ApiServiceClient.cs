using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lib;
using System.Net.Http.Json;
using Azure.Data.AppConfiguration;
using System.Text.Json;

public class ApiServiceClient : ServiceClient
{
    public ApiServiceClient(HttpClient httpClient) : base(httpClient)
    {
    }

    public async Task<List<Image>> GetImages()
    {
        return await base.httpClient.GetFromJsonAsync<List<Image>>("/images");
    }

    public async Task<Image> GetImageAsync(string id)
    {
        try
        {
            return await base.httpClient.GetFromJsonAsync<Image>($"/image/{id}");
        }
        catch
        {
            return null;
        }
    }

    public async Task<Image> PostImage(ImageType type, string url)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true };
        var image = new Image() { Type = type };
        if (image.Type == ImageType.New)
        {
            image.Url = url;
        }
        var response = await base.httpClient.PostAsJsonAsync("/image", image, options);
        image = await response.Content.ReadFromJsonAsync<Image>(options);
        image.Status = response.StatusCode.ToString().ToLower() == "ok" ? "Processing..." : "Upload failed, try again.";
        image.Sentiment = "loading";
        image.Text = "Loading...";
        return image;

        // TODO: Improve messaging to UI
    }

    public async Task<string> GetBorderStyle()
    {
        var borderStyle = "solid";
        try
        {
            var borderStyleSetting = await base.httpClient.GetFromJsonAsync<ConfigurationSetting>("/config?name=borderStyle");
            borderStyle = borderStyleSetting.Value;
        }
        catch { }
        return borderStyle;
    }
}