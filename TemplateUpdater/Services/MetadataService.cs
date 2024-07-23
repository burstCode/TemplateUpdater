using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using TemplateUpdater.Models;

namespace TemplateUpdater.Services
{
    /*
     * Сервис для работы с метаданными.
     * Хранит данные о текущих шаблонах в том же формате, что и база данных.
    */
    public class MetadataService
    {
        private readonly string _metadataFilePath;

        public MetadataService(string metadataFilePath)
        {
            _metadataFilePath = metadataFilePath;
        }

        public async Task<List<Template>> LoadMetadataAsync()
        {
            if (!File.Exists(_metadataFilePath))
            {
                return new List<Template>();
            }

            var json = await File.ReadAllTextAsync(_metadataFilePath);
            return JsonSerializer.Deserialize<List<Template>>(json);
        }

        public async Task SaveMetadataAsync(List<Template> metadata)
        {
            var json = JsonSerializer.Serialize(metadata);
            await File.WriteAllTextAsync(_metadataFilePath, json);
        }
    }
}
