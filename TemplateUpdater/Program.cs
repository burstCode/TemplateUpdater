using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TemplateUpdater.Models;
using TemplateUpdater.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var templateClient = new TemplateClient(httpClient);
        var localTemplateDirectory = "";
        var metadataFilePath = Path.Combine(localTemplateDirectory, "templates_metadata.json");

        var metadataService = new MetadataService(metadataFilePath);
        var localMetadata = await metadataService.LoadMetadataAsync();

        // Получение списка актуальных шаблонов
        var templates = await templateClient.GetTemplatesAsync();

        // Проверка и обновление шаблонов
        foreach (var template in templates)
        {
            var localTemplatePath = Path.Combine(localTemplateDirectory, template.TemplateFilename);
            var localTemplate = localMetadata.FirstOrDefault(t => t.Id == template.Id);

            if (localTemplate == null || localTemplate.LastUpdated < template.LastUpdated)
            {
                Console.WriteLine($"Найдено обновление для шаблона №: {template.Id}");
                await UpdateTemplateAsync(templateClient, template, localTemplatePath);
                UpdateLocalMetadata(localMetadata, template);
            }
            else
            {
                Console.WriteLine($"Шаблон №{template.Id} не требует обновления.");
            }
        }

        await metadataService.SaveMetadataAsync(localMetadata);
    }

    // Обновление шаблона
    private static async Task UpdateTemplateAsync(TemplateClient templateClient, Template template, string localTemplatePath)
    {
        var templateData = await templateClient.DownloadTemplateAsync(template.Id);
        await File.WriteAllBytesAsync(localTemplatePath, templateData);
        File.SetLastWriteTime(localTemplatePath, template.LastUpdated);
        Console.WriteLine($"Шаблон №{template.Id} успешно обновлён.");
    }

    // Обновление локальных метаданных
    private static void UpdateLocalMetadata(List<Template> localMetadata, Template template)
    {
        var existingTemplate = localMetadata.FirstOrDefault(t => t.Id == template.Id);
        if (existingTemplate != null)
        {
            existingTemplate.LastUpdated = template.LastUpdated;
            existingTemplate.TemplateFilename = template.TemplateFilename;
        }
        else
        {
            localMetadata.Add(new Template
            {
                Id = template.Id,
                TemplateFilename = template.TemplateFilename,
                LastUpdated = template.LastUpdated
            });
        }
    }
}
