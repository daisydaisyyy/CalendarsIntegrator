using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using CalendarsIntegrator.Dependencies.Concretes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator
{
    internal static class Services
    {

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
            var sc = new ServiceCollection();
            var items = ConfigHandler.configuration();

            string tenantId = items[0].ToString();
            string clientId = items[1].ToString();
            string clientSecret = items[2].ToString();
            string scopes = items[3].ToString();

            sc.AddSingleton<IGraphClient>(sp => new GraphClient(
                tenantId,
                clientId,
                clientSecret,
                scopes
            ));
            sc.AddSingleton<IHDAClient>(sp => new HDAClient());

            return sc.BuildServiceProvider();
        }


    }
}
