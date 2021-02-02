using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Lib.Model;
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

    public async Task<HttpResponseMessage> DeleteImage(string id)
    {
        return await base.httpClient.DeleteAsync($"/image/{id}");
    }

    public async Task<Image> PostImage(Image image)
    {
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, IgnoreNullValues = true };
        image.Status = "Uploading...";
        var response = await base.httpClient.PostAsJsonAsync("/image", image, options);
        image = await response.Content.ReadFromJsonAsync<Image>(options);
        image.Status = response.StatusCode.ToString().ToLower() == "ok" ? "Processing..." : "Upload failed, try again.";
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