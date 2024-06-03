using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.IO;

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

    public async Task<HttpResponseMessage> PostAsync(string endpoint, MultipartFormDataContent form)
    {
        var response = await _httpClient.PostAsync($"{_apiBaseUrl}{endpoint}", form);
        response.EnsureSuccessStatusCode();
        return response;
    }

    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        var response = await _httpClient.DeleteAsync($"{_apiBaseUrl}{endpoint}");
        response.EnsureSuccessStatusCode();
        return response;
    }

    // Yeni dosya indirme metodu
    public async Task<Stream> GetFileStreamAsync(string endpoint)
    {
        var response = await _httpClient.GetAsync($"{_apiBaseUrl}{endpoint}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStreamAsync();
    }
}
