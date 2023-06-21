using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Graph.IdentityGovernance.EntitlementManagement;
using Microsoft.Graph.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.WebSockets;
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
            // done
            try
            {
                var previous = allEventsList;
                allEventsList.Clear();
                foreach (var email in search.Emails)
                {
                    var calendars = await graphClient.Client.Users[email]
                       .Calendars.GetAsync();

                    var defaultCalendar = calendars?.Value?.Where(c => string.Equals(c.Name, "Calendar")).FirstOrDefault();

                  

                    if (defaultCalendar == null) return;



                    var events = await graphClient.Client.Users[email]
                        .Calendars[defaultCalendar.Id]
                        .Events
                        .GetAsync( rq => rq.QueryParameters.Top = 999);

                    List<Microsoft.Graph.Models.Event> userEventsList = events?.Value;
                    if (userEventsList == null) return;

                    // convert to CalendarEntry and add events to allEventsList
                    userEventsList.ForEach(e => { allEventsList.Add(e); }) ;

                }

          //      Console.WriteLine(allEventsList.Count);
              
            }
            catch (Exception ex)
            {
                throw;
            }



        }

        public async Task<bool> Exists(ICalendarEntry entry)
        {
            // done

            //           var foundEvent = allEventsList.Find(e =>
            //             DateTime.Parse(e.Start.DateTime).Equals(entry.Start) &&
            //         DateTime.Parse(e.End.DateTime).Equals(entry.End) &&
            //          e.Subject.Equals(entry.Subject)
            //     );

            /*         if (DateTime.Parse(e.Start.DateTime).ToString().Equals(entry.Start) && DateTime.Parse(e.End.DateTime).ToString().Equals(entry.End) && e.Subject.ToString().Equals(entry.Subject) && entry.Email.Equals(e.Organizer.EmailAddress.Address.ToString()))*/


            CalendarEntry foundEvent = null;

         

            foreach (var item in allEventsList)
            {
                var e = convertGraphEvent(item);

                if (e.Email.Equals(entry.Email) && e.Subject.Equals(entry.Subject) && e.Start.Equals(entry.Start) && e.End.Equals(entry.End))
                {
                    foundEvent = (CalendarEntry)e;
                    break;
                }
            }
            bool found = foundEvent != null;

            return found;
        }

        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            List<ICalendarEntry> entries = new List<ICalendarEntry>();
            allEventsList.ForEach(e => { entries.Add(convertGraphEvent(e)); });

            //done
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
        }

        public Task Delete(ICalendarEntry entry)
        {
            // to be done

            var deletedEvent = new Microsoft.Graph.Models.Event
            {
                Subject = entry.Subject,
                Start = new Microsoft.Graph.Models.DateTimeTimeZone
                {
                    DateTime = entry.Start.ToString("o")
                },
                End = new Microsoft.Graph.Models.DateTimeTimeZone
                {
                    DateTime = entry.End.ToString("o")
                 }

             };

            
            if(entry.Subject.Equals("fra"))
            {
                Console.WriteLine("fra");
            }

            if (entry.Email.Equals("USER_MAIL1", StringComparison.InvariantCultureIgnoreCase))
                entry.Email = "ADMIN_TEST_MAIL";


            if (entry.Email.Equals("USER_MAIL2", StringComparison.InvariantCultureIgnoreCase))
                entry.Email = "TEST_MAIL";
            TimeZoneInfo italyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Rome");

            var eventToDelete = allEventsList.FirstOrDefault(e =>
            {
                var eventStart = TimeZoneInfo.ConvertTime(DateTime.Parse(e.Start.DateTime), italyTimeZone);

                var eventEnd = TimeZoneInfo.ConvertTime(DateTime.Parse(e.End.DateTime), italyTimeZone);
              //  Console.WriteLine(entry.Start.ToUniversalTime().ToString());
              //  Console.WriteLine(eventStart.ToString());

                return e.Subject.Equals(entry.Subject) &&
                       eventStart.ToString().Equals(entry.Start.ToUniversalTime().ToString()) &&
                       eventEnd.ToString().Equals(entry.End.ToUniversalTime().ToString());
            });


            if (eventToDelete != null) 
            { var eventRequest = graphClient.Client.Users[entry.Email].Calendar.Events[eventToDelete.Id].DeleteAsync();
                return Task.FromResult(eventRequest);
            }
            return Task.CompletedTask;

           
           
        }


        public CalendarEntry convertGraphEvent(Microsoft.Graph.Models.Event graphEvent)
        {
            var startDateTimeString = graphEvent.Start.DateTime;
            var endDateTimeString = graphEvent.End.DateTime;

            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Rome"); // timezone identifier for Italy

            var Start = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(startDateTimeString), timezone);
            var End = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(endDateTimeString), timezone);
            var Email = graphEvent.Organizer.EmailAddress.Address;
            var Subject = graphEvent.Subject;
            var Body = graphEvent.Body.Content;
            var Location = "";
            CalendarEntry calendarEntry = new CalendarEntry(Start, End, Email, Subject, Body, Location);
            return calendarEntry;
        }




    }
}
