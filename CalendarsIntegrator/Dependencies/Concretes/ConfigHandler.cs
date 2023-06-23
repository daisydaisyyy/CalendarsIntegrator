using CalendarsIntegrator.Dependencies.Concretes;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;

namespace CalendarsIntegrator.Core.Concretes
{
    public static class ConfigHandler
    {
        public static List<object> configuration()
        {
            var fieldNames = typeof(GraphClient)
          .GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
          .Select(f => f.Name);
            fieldNames.Take(fieldNames.Count() - 1).ToList();

            var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var directoryPath = Path.Combine(desktopPath, "Stage 2023", "CalendarsIntegrator");
            var filePath = Path.Combine(directoryPath, "configurationFile.json");

            var configBuilder = new ConfigurationBuilder().AddJsonFile(filePath);

            var json = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var jsonDocument = JsonDocument.Parse(json);

            // Access properties dynamically
            var items = new List<object>();
            foreach (var field in fieldNames)
            {
                if (field == "scopes") // Check if the field is "scopes"
                {
                    if (jsonDocument.RootElement.TryGetProperty(field, out var property))
                    {
                        if (property.ValueKind == JsonValueKind.Array)
                        {
                            var scopes = property.EnumerateArray()
                                                 .Select(s => s.GetString())
                                                 .ToList();
                            items.AddRange(scopes);
                        }
                    }
                    else
                    {
                        Console.WriteLine("An error has occured: there was an error reading the json configuration file");
                        Environment.Exit(0);
                    }
                }
                else // Handle other fields
                {
                    if (jsonDocument.RootElement.TryGetProperty(field, out var property))
                    {
                        if (property.ValueKind == JsonValueKind.String)
                        {
                            items.Add(property.GetString());
                        }
                    }
                }


            }
            return items;
        }


    }
}

