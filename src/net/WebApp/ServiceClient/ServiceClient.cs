using System.Net.Http;

public class ServiceClient
{
    protected readonly HttpClient httpClient;

    public ServiceClient(HttpClient httpClient)
    {
        this.httpClient = httpClient;
    }
}