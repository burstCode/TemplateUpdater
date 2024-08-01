using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using TemplateUpdater.Models;
using TemplateUpdater.Services;

class Program
{
    static async Task Main(string[] args)
    {
        var httpClient = new HttpClient();
        var host = CreateHostBuilder(args).Build();
        var templateClient = host.Services.GetRequiredService<TemplateClient>();

        // Инициализируем папочку для шаблонов
        FileService.EnsureUploadsFolderExists();

        var localTemplateDirectory = Path.Combine(Directory.GetCurrentDirectory(), "LocalTemplates");
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
                // Удаляем устаревший файл
                if (localTemplate != null)
                {
                    File.Delete(Path.Combine(localTemplateDirectory, localTemplate.TemplateFilename));
                }

                Console.WriteLine($"Найдено обновление для шаблона №{template.Id}");

                var content = await templateClient.DownloadTemplateAsync(template.Id);
                var filePath = Path.Combine("LocalTemplates", template.TemplateFilename);
                await File.WriteAllBytesAsync(filePath, content);

                UpdateLocalMetadata(localMetadata, template);

                Console.WriteLine($"Шаблон №{template.Id} успешно обновлён.");
            }
            else
            {
                Console.WriteLine($"Шаблон №{template.Id} не требует обновления.");
            }
        }

        await metadataService.SaveMetadataAsync(localMetadata);
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("C:\\Users\\mikex\\OneDrive\\Рабочий стол\\EGISZ_Templates_Project\\TemplateUpdater\\TemplateUpdater\\appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services.AddHttpClient<TemplateClient>(client =>
                {
                    client.BaseAddress = new Uri(context.Configuration["ApiSettings:BaseUrl"]);
                });
                services.AddSingleton<TemplateClient>();
            });

    // Обновление локальных метаданных
    private static void UpdateLocalMetadata(List<Template> localMetadata, Template template)
    {
        var existingTemplate = localMetadata.FirstOrDefault(t => t.Id == template.Id);
        if (existingTemplate != null)
        {
            existingTemplate.LastUpdated = template.LastUpdated;
            existingTemplate.TemplateFilename = template.TemplateFilename;
            existingTemplate.Version = template.Version;
        }
        else
        {
            localMetadata.Add(new Template
            {
                Id = template.Id,
                TemplateFilename = template.TemplateFilename,
                LastUpdated = template.LastUpdated,
                Version = template.Version
            });
        }
    }
}
