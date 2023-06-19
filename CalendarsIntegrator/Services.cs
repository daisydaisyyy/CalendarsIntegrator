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

            sc.AddSingleton<IGraphClient>(sp => new GraphClient());
            sc.AddSingleton<IHDAClient>(sp => new HDAClient());

            return sc.BuildServiceProvider();

        }

    }
}
