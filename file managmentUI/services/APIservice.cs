using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

public class APIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiBaseUrl;

    public APIService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _apiBaseUrl = configuration["ApiBaseUrl"];
    }

    public async Task<T> GetAsync<T>(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_apiBaseUrl}{endpoint}");
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(responseString);
    }

    public async Task<HttpResponseMessage> PostAsync<T>(string endpoint, T data)
    {
        var response = await _httpClient.PostAsJsonAsync($"{_apiBaseUrl}{endpoint}", data);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}{endpoint}");
        response.EnsureSuccessStatusCode();
        return response;
    }
}
