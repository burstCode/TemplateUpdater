using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TemplateUpdater.Models;

namespace TemplateUpdater.Services
{
    /*
     * Сервис для работы с API 
    */
    public class TemplateClient
    {
        private readonly HttpClient _httpClient;
        private string _baseUrl = "http://localhost:5000/api/templates";

        public TemplateClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Получение списка шаблонов
        public async Task<List<Template>> GetTemplatesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Template>>($"{_baseUrl}");
        }

        // Получение данных о конкретном шаблоне
        public async Task<List<Template>> GetTemplateAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<List<Template>>($"{_baseUrl}/{id}");
        }

        // Загрузка шаблона
        public async Task<byte[]> DownloadTemplateAsync(int id)
        {
            var response = await _httpClient.GetAsync($"{_baseUrl}/download/{id}");
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}
