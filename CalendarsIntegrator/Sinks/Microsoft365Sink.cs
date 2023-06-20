using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace CalendarsIntegrator.Sinks
{
    internal class Microsoft365Sink : ISink
    {

            private IGraphClient graphClient;
            private List<Microsoft.Graph.Models.Event> allEventsList = new List<Microsoft.Graph.Models.Event>();


        public Microsoft365Sink()
        {
            graphClient = Services.ServiceCollection.GetRequiredService<IGraphClient>();
        }

        public async Task Load(ISearch search)
        {
            try
             { 

                /*
                Example of load
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

                // done
                

                foreach (var email in search.Emails)
                {
                    var calendars = await graphClient.Client.Users[email]
                       .Calendars.GetAsync();

                    var defaultCalendar = calendars?.Value?.Where(c => string.Equals(c.Name, "Calendar")).FirstOrDefault();

                    if (defaultCalendar == null) return;

                    var events = await graphClient.Client.Users[email]
                        .Calendars[defaultCalendar.Id]
                        .Events.GetAsync();

                    List<Microsoft.Graph.Models.Event>? userEventsList = events?.Value;
                    if (userEventsList == null) return;

                    this.allEventsList.AddRange(userEventsList);

                    for (int i = 0; i < userEventsList.Count; i++)
                    {
                        var item = userEventsList[i];
                        //Console.WriteLine($"From {item.Start?.DateTime} to {item.End?.DateTime} Subject: {item.Subject}");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }


        }

        public Task<bool> Exists(ICalendarEntry entry)
        {
            // done
            foreach(Microsoft.Graph.Models.Event e in allEventsList)
            {
                Console.WriteLine(DateTime.Parse(e.Start.DateTime).ToString());
                Console.WriteLine("entry: ", ((CalendarEntry)entry).Start.ToString());
                break;


            }

            var foundEvent = allEventsList.Find(e =>
                DateTime.Parse(e.Start.DateTime).Equals(entry.Start) &&
                DateTime.Parse(e.End.DateTime).Equals(entry.End) &&
                e.Subject.Equals(entry.Subject)
            );

            bool found = foundEvent != null;

            return Task.FromResult(found);
            //throw new NotImplementedException();
        }

        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            //done

            List<ICalendarEntry> entries = new List<ICalendarEntry>();

            foreach (Microsoft.Graph.Models.Event graphEvent in allEventsList)
            {
                var Start = DateTime.Parse(graphEvent.Start.DateTime);
                var End = DateTime.Parse(graphEvent.End.DateTime);
                var Email = graphEvent.Organizer.EmailAddress.Address;
                var Subject = graphEvent.Subject;
                var Body = graphEvent.Body.Content;
                var Location = "";
                CalendarEntry calendarEntry = new CalendarEntry(Start,End,Email,Subject,Body,Location);
                entries.Add(calendarEntry);
            


            };

            
           
            return Task.FromResult((IEnumerable<ICalendarEntry>)entries);
        }

        public async Task Insert(ICalendarEntry entry)
        {
            // to be done

             var newEvent = new Microsoft.Graph.Models.Event
             {
                 Subject = entry.Subject,
                 Start = new Microsoft.Graph.Models.DateTimeTimeZone
                 {
                     DateTime = entry.Start.ToString("o"),
                     TimeZone = TimeZoneInfo.Local.Id
                 },
                 End = new Microsoft.Graph.Models.DateTimeTimeZone
                 {
                     DateTime = entry.End.ToString("o"),
                     TimeZone = TimeZoneInfo.Local.Id
                 }

             };

            var eventRequest = graphClient.Client.Users[entry.Email].Calendar.Events;
            await eventRequest.PostAsync(newEvent);



            //throw new NotImplementedException();
        }

        public async Task Delete(ICalendarEntry entry)
        {
            // to be done

            var deletedEvent = new Microsoft.Graph.Models.Event
             {
                 Subject = entry.Subject,
                 Start = new Microsoft.Graph.Models.DateTimeTimeZone
                 {
                     DateTime = entry.Start.ToString("o"),
                     TimeZone = TimeZoneInfo.Local.Id
                 },
                 End = new Microsoft.Graph.Models.DateTimeTimeZone
                 {
                     DateTime = entry.End.ToString("o"),
                     TimeZone = TimeZoneInfo.Local.Id
                 }

             };


            var eventRequest = graphClient.Client.Users[entry.Email].Calendar.Events[deletedEvent.Id];
            await eventRequest.DeleteAsync();

            //throw new NotImplementedException();
        }





    }
}
