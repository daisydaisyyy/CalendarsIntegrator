using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CalendarsIntegrator.Core.Concretes
{
    public static class ConfigHandler
    {


        public static Dictionary<string, string> configuration()
        {
            ConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string directoryPath = Path.Combine(desktopPath, "Stage 2023", "CalendarsIntegrator");
            string filePath = Path.Combine(directoryPath, "configurationFile.json");

            configurationBuilder.AddJsonFile(filePath);

            IConfigurationRoot configuration = configurationBuilder.Build();

            var temp = configuration.GetChildren();
            var dictionary = configuration.GetChildren()
            .ToDictionary(section => section.Key, section => section.Value);
            

            
            if (dictionary.Count == 0)
            {
                Services._logger.LogError("An error has occurred: there was an error reading the JSON configuration file");
                throw new Exception();
            }

            return dictionary;
        }

    }
}

