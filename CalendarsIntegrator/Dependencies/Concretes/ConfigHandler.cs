using Microsoft.Extensions.Configuration;
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
                Console.WriteLine("An error has occurred: there was an error reading the JSON configuration file");
                Environment.Exit(0);
            }

            return dictionary;
        }

    }
}

