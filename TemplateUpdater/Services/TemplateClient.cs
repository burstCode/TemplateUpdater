using System.Net.Http.Json;
using TemplateUpdater.Models;
using Microsoft.Extensions.Configuration;

namespace TemplateUpdater.Services
{
    /*
     * Сервис для работы с API 
    */
    public class TemplateClient
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _apiKey;

        public TemplateClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;

            _baseUrl = configuration["ApiSettings:BaseUrl"];
            _apiKey = configuration["ApiSettings:ApiKey"];
        }

        // Получение списка шаблонов
        public async Task<List<Template>> GetTemplatesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}");
            request.Headers.Add("Api-Key", _apiKey);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Template>>();
        }

        // Получение данных о конкретном шаблоне
        public async Task<List<Template>> GetTemplateAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/{id}");
            request.Headers.Add("Api-Key", _apiKey);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<List<Template>>();
        }

        // Загрузка шаблона
        public async Task<byte[]> DownloadTemplateAsync(int id)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{_baseUrl}/download/{id}");
            request.Headers.Add("Api-Key", _apiKey);
            var response = await _httpClient.SendAsync(request);

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
