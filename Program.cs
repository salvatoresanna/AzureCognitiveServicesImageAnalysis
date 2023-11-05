using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using AzureCognitiveServicesImageAnalysis_ConsoleApp.Service;
using System.Text.Json;

namespace AzureCognitiveServicesImageAnalysis_ConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var config = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appSettings.json", optional: true, reloadOnChange: true)
           .Build();

            LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
            var logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
            logger.Info($"Starting application...");

            var endpoint = config.GetSection("Settings").GetValue<string>("VISION_ENDPOINT");
            var key = config.GetSection("Settings").GetValue<string>("VISION_KEY");
            var filePath = config.GetSection("Settings").GetValue<string>("FILE_PATH");

            var serviceProvider = new ServiceCollection()
            .AddLogging(builder => { builder.AddNLog(); })
            .AddSingleton<IComputerVisionClient>(x => new ComputerVisionClient(new ApiKeyServiceClientCredentials(key)) { Endpoint = endpoint })
            .AddSingleton<IAIServiceAnalysis>(x => new AIServiceAnalysis(x.GetRequiredService<IComputerVisionClient>()))
            .AddSingleton<IAIServiceObjects>(x => new AIServiceObjects(x.GetRequiredService<IComputerVisionClient>()))
            .AddSingleton<IAIServiceRead>(x => new AIServiceRead(x.GetRequiredService<IComputerVisionClient>()))
            .AddSingleton<IAIServiceScan>(x => new AIServiceScan(x.GetRequiredService<IAIServiceAnalysis>(), x.GetRequiredService<IAIServiceObjects>(), x.GetRequiredService<IAIServiceRead>()))
            .BuildServiceProvider();

            var service = serviceProvider.GetService<IAIServiceScan>();
            var fileScanObject = service.ScanFile(filePath).GetAwaiter().GetResult();
            var serializedfileScanObject = JsonSerializer.Serialize(fileScanObject);

            logger.Info($"Closing application...");
        }
    }
}