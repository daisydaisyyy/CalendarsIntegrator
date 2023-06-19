using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Sinks
{
    internal class HDAActivitySink : ISink
    {

        private IHDAClient hdaClient;

        public HDAActivitySink()
        {
            hdaClient = Services.ServiceCollection.GetRequiredService<IHDAClient>();
        }

        public Task Load(ISearch search)
        {
            /*
             Example of reading from hdaClient

            var test = hdaClient.GetActivities(search.Emails, search.From, search.To);
            if (test == null) return Task.CompletedTask;

            foreach (DataRow activity in test.Rows)
            {
                Console.WriteLine(activity.ItemArray);
            }
            */

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            // to be done
            throw new NotImplementedException();
        }

        public Task<bool> Exists(ICalendarEntry entry)
        {
            // to be done
            throw new NotImplementedException();
        }

       

        public Task Insert(ICalendarEntry entry)
        {
            throw new NotImplementedException("I'm a readonly sink");
        }

        public Task Delete(ICalendarEntry entry)
        {
            throw new NotImplementedException("I'm a readonly sink");
        }


     
    }
}
