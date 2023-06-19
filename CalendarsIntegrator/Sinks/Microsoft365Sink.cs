using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Sinks
{
    internal class Microsoft365Sink : HDAActivitySink
    {

        private IGraphClient graphClient;

        public Microsoft365Sink()
        {
            graphClient = Services.ServiceCollection.GetRequiredService<IGraphClient>();
        }

        public async Task Load(ISearch search)
        {
            try
            {


                // to be done

                /*
                Example of loead
                 foreach (var email in search.Emails)
                 {
                     var calendars = await graphClient.Client.Users[email]
                        .Calendars.GetAsync();

                     var defaultCalendar = calendars?.Value?.Where(c => string.Equals(c.Name, "Calendar")).FirstOrDefault();

                     if (defaultCalendar == null) return;

                     var events = await graphClient.Client.Users[email]
                         .Calendars[defaultCalendar.Id]
                         .Events.GetAsync();

                     List<Microsoft.Graph.Models.Event>? list = events?.Value;
                     if (list == null) return;

                     for (int i = 0; i < list.Count; i++)
                     {
                         var item = list[i];
                         Console.WriteLine($"Da {item.Start?.DateTime} to {item.End?.DateTime} subject {item.Subject}");
                     }
                 }
                */



            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public Task<bool> Exists(ICalendarEntry entry)
        {
            // to be done
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            // to be done
            throw new NotImplementedException();
        }

        public Task Insert(ICalendarEntry entry)
        {
            // to be done
            throw new NotImplementedException();
        }

        public Task Delete(ICalendarEntry entry)
        {
            // to be done
            throw new NotImplementedException();
        }





    }
}
