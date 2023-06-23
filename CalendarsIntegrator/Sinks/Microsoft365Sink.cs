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
            private List<Microsoft365CalendarEntry> allEventsList = new List<Microsoft365CalendarEntry>();


        public Microsoft365Sink()
        {
            graphClient = Services.ServiceCollection.GetRequiredService<IGraphClient>();
        }


        // to optimize
        public async Task Load(ISearch search)
        {
            // done
            try
            {
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
                    if (userEventsList == null) continue;

                    // convert to CalendarEntry and add events to allEventsList
                    userEventsList.ForEach(e => { allEventsList.Add(convertGraphEvent(e)); }) ;

                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // to optimize
        public async Task<bool> Exists(ICalendarEntry entry)
        {
            // done
            CalendarEntry foundEvent = null;

            foreach (var e in allEventsList)
            {
                if (e.Email == entry.Email && e.Subject == entry.Subject && e.Start.ToString() == entry.Start.ToString() && e.End.ToString() == entry.End.ToString())
                {
                    foundEvent = e;
                    break;
                }
            }
            bool found = foundEvent != null;

            return found;
        }

        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            List<ICalendarEntry> entries = new List<ICalendarEntry>();
            allEventsList.ForEach(e => { entries.Add(e); });

            //done
            return Task.FromResult((IEnumerable<ICalendarEntry>)entries);
        }

        public async Task Insert(ICalendarEntry entry)
        {
            // done

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

        public async Task Delete(ICalendarEntry entry)
        {
            // done

            entry.Email = entry.Email.Equals("USER_MAIL1", StringComparison.InvariantCultureIgnoreCase)
             ? "ADMIN_TEST_MAIL"
             : entry.Email;

            entry.Email = entry.Email.Equals("USER_MAIL2", StringComparison.InvariantCultureIgnoreCase)
            ? "TEST_MAIL"
            : entry.Email;


            // find event in allEventList, get the event id and delete it

            var eventToDelete = allEventsList.FirstOrDefault(e =>
            {
                return e.Subject.Equals(entry.Subject) &&
                       e.Start.ToString().Equals(entry.Start.ToString()) &&
                       e.End.ToString().Equals(entry.End.ToString());
            });


            if (eventToDelete != null) 
            {
                if(eventToDelete.Subject.Equals(entry.Subject) &&
                    eventToDelete.Start.ToString().Equals(entry.Start.ToString()) &&
                    eventToDelete.End.ToString().Equals(entry.End.ToString()) && 
                    entry.Email.Equals(eventToDelete.Email.ToString()))
                { 
                    await graphClient.Client.Users[entry.Email].Events[eventToDelete.Id].DeleteAsync();
                    allEventsList.Remove(eventToDelete);
                   //  Console.WriteLine("deleted" + entry.Start);
                }
            }
           
        }


        public Microsoft365CalendarEntry convertGraphEvent(Microsoft.Graph.Models.Event graphEvent)
        {
            var startDateTimeString = graphEvent.Start.DateTime;
            var endDateTimeString = graphEvent.End.DateTime;
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Rome");
            var Start = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(startDateTimeString), timezone);
            var End = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(endDateTimeString), timezone);
            var Email = graphEvent.Organizer.EmailAddress.Address;
            var Subject = graphEvent.Subject;
            var Body = graphEvent.Body.Content;
            var Location = "";
            var id = graphEvent.Id;

            Microsoft365CalendarEntry calendarEntry = new Microsoft365CalendarEntry(Start, End, Email, Subject, Body, Location, id);
            return calendarEntry;
        }




    }
}
