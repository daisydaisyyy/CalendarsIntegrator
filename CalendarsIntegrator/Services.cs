using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using CalendarsIntegrator.Dependencies.Concretes;
using CalendarsIntegrator.Sinks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace CalendarsIntegrator
{
    internal static class Services
    {
        public static ILogger<string> _logger;
        private static ServiceProvider? _Services;

        public static ServiceProvider ServiceCollection
        {
            get
            {
                if (_Services == null)
                {
                    _Services = RegisterServices();
                }

                return _Services;
            }
        }

        private static ServiceProvider RegisterServices()
        {
            try
            {
                ServiceCollection sc = new ServiceCollection();
                var items = ConfigHandler.configuration(); // read jsonfile

                sc.AddSingleton<IGraphClient>(sp => new GraphClient(
                    items["tenantId"].ToString(),
                    items["clientId"].ToString(),
                    items["clientSecret"].ToString(),
                    JsonSerializer.Deserialize<string[]>(items["scopes"].ToString())
                ));
                sc.AddSingleton<IHDAClient>(sp => new HDAClient(
                    items["dataSource"].ToString(),
                    items["userID"].ToString(),
                    items["password"].ToString(),
                    items["initialCatalog"].ToString(),
                    bool.Parse(items["encrypt"].ToString())
               ));

                return sc.BuildServiceProvider();
            }
            catch(Exception ex)
            {
                Environment.Exit(0);
                return null;
            }
        }


    }
}
