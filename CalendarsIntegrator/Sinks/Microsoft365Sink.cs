using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using System.Data;
using System.Drawing.Printing;

namespace CalendarsIntegrator.Sinks
{
    public class Microsoft365Sink : ISink
    {

        private IGraphClient graphClient;
        private List<Microsoft365CalendarEntry> allEventsList = new List<Microsoft365CalendarEntry>();
        private readonly ILogger<string> _logger;

        public Microsoft365Sink(ILogger<string> logger)
        {
            graphClient = Services.ServiceCollection.GetRequiredService<IGraphClient>();
            var a = addExtensions();
            var b = Task.FromResult(a);

            _logger = logger;

        }

        


        public async Task<ExtensionProperty> addExtensions()
        {
            var requestBody = new ExtensionProperty
            {
                Name = "jobGroupTracker",
                DataType = "String",
                TargetObjects = new List<string>
                {
                    "User",
                },
            };
            var result = await graphClient.Client.Applications["EXAMPLE_KEY1"].ExtensionProperties.PostAsync(requestBody);
            return result;
        }

        public async Task Load(ISearch search)
        {
            // done
            allEventsList.Clear();
            try
            {
                foreach (var email in search.Emails)
                {
                    var calendars = await graphClient.Client.Users[email].Calendars.GetAsync();

                    var defaultCalendar = calendars?.Value?.Where(c => string.Equals(c.Name, "Calendar")).FirstOrDefault();

                    if (defaultCalendar == null) return;

                    var events = await graphClient.Client.Users[email]
                        .Calendars[defaultCalendar.Id]
                        .Events
                        .GetAsync(rq => rq.QueryParameters.Top = 999);

                    List<Event> userEventsList = events?.Value;
                    if (userEventsList == null) continue;

                    // convert to CalendarEntry and add events to allEventsList
                    userEventsList.ForEach(e => { allEventsList.Add(convertGraphEvent(e));
                        _logger.LogInformation("Loaded event, details: " + "Email: " + convertGraphEvent(e).Email + " |Start: " + convertGraphEvent(e).Start + " |End: " + convertGraphEvent(e).End + " |Subject: " + convertGraphEvent(e).Subject + " |Id: " + convertGraphEvent(e).Id + " |DbID: " + convertGraphEvent(e).DbID + "| ", AppLogEvents.Read); });   

                }
            }
            catch (Azure.Identity.AuthenticationFailedException authEx)
            {
                _logger.LogError("The load method from the calendar generated an exception due to an authentication error, check auth keys on configurationFile.json",AppLogEvents.NotRead);
                Thread.Sleep(3000);
                Environment.Exit(0);
            }
            catch (Exception e)
            {
                _logger.LogError("The load method from the calendar generated an exception, details: " + e.StackTrace, AppLogEvents.NotRead);
            }
        }

        public async Task<bool> Exists(ICalendarEntry entry)
        {
            bool found = allEventsList.Any(e =>
                e.Email == entry.Email &&
                e.Subject == entry.Subject &&
                e.Start.ToString() == entry.Start.ToString() &&
                e.End.ToString() == entry.End.ToString());

            return found;
        }


        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            // done
            List<ICalendarEntry> entries = new List<ICalendarEntry>();
            allEventsList.ForEach(e => { entries.Add(e); });

            return Task.FromResult((IEnumerable<ICalendarEntry>)entries);
        }

        public async Task Insert(ICalendarEntry entry)
        {
            // done
            try
            {
                var newEvent = new Event
                {
                    Subject = entry.Subject,
                    Start = new DateTimeTimeZone
                    {
                        DateTime = entry.Start.ToString("o"),
                        TimeZone = TimeZoneInfo.Local.Id
                    },
                    End = new DateTimeTimeZone
                    {
                        DateTime = entry.End.ToString("o"),
                        TimeZone = TimeZoneInfo.Local.Id
                    }
                };

                newEvent.TransactionId = entry.DbID; 
              
                
                var eventRequest = graphClient.Client.Users[entry.Email].Calendar.Events;
                await eventRequest.PostAsync(newEvent);
                _logger.LogInformation("Inserted event, details: " + "Email: " + entry.Email + " |Start: " + entry.Start + " |End: " + entry.End + " |Subject: " + entry.Subject + " |Id: DOESN'T HAVE AN ID YET" + " |DbID: " + entry.DbID + "| ", AppLogEvents.Create);

            }
            catch(Exception e)
            {
                _logger.LogError("The insert method generated an exception, details: " + e.StackTrace, AppLogEvents.NotCreated);
            }
        

           
        }

        public async Task Delete(ICalendarEntry entry)
        {
                // done
            try
            {

                entry.Email = entry.Email.Equals("USER_MAIL1", StringComparison.InvariantCultureIgnoreCase)
                ? "ADMIN_TEST_MAIL"
                : entry.Email;

                entry.Email = entry.Email.Equals("USER_MAIL2", StringComparison.InvariantCultureIgnoreCase)
                ? "TEST_MAIL"
                : entry.Email;


                // find event in allEventList, get the event id and delete it

                Microsoft365CalendarEntry eventToDelete = allEventsList.FirstOrDefault(e =>
                {
                    return e.Subject.Equals(entry.Subject) &&
                           e.Start.ToString().Equals(entry.Start.ToString(), StringComparison.Ordinal) &&
                           e.End.ToString().Equals(entry.End.ToString(), StringComparison.Ordinal);
                });


                if (eventToDelete != null)
                {
                    if (eventToDelete.Subject.Equals(entry.Subject) &&
                        eventToDelete.Start.ToString().Equals(entry.Start.ToString()) &&
                        eventToDelete.End.ToString().Equals(entry.End.ToString()) &&
                        entry.Email.Equals(eventToDelete.Email.ToString()))
                    {
                        await graphClient.Client.Users[entry.Email].Events[eventToDelete.Id].DeleteAsync();
                        allEventsList.Remove(eventToDelete);
                    }
                }
                _logger.LogInformation("Deleted event, details: " + "Email: " + eventToDelete.Email + " |Start: " + eventToDelete.Start + " |End: " + eventToDelete.End + " |Subject: " + eventToDelete.Subject + " |Id: " + eventToDelete.Id + " |DbID: " + eventToDelete.DbID + "| ", AppLogEvents.Delete);
            }
            catch(Exception ex)
            {
                _logger.LogError("The delete method generated an exception, details: " + ex.StackTrace, AppLogEvents.NotDeleted);
            }
        }

        public Microsoft365CalendarEntry convertGraphEvent(Event graphEvent)
        {
            var startDateTimeString = graphEvent.Start.DateTime;
            var endDateTimeString = graphEvent.End.DateTime;
            var timezone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Rome");
            DateTime Start = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(startDateTimeString), timezone);
            DateTime End = TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(endDateTimeString), timezone);
            string Email = graphEvent.Organizer.EmailAddress.Address;
            string Subject = graphEvent.Subject;
            string Body = graphEvent.Body.Content;
            string Location = "";
            string id = graphEvent.Id;
            string dbid = graphEvent.TransactionId;

            Microsoft365CalendarEntry calendarEntry = new Microsoft365CalendarEntry(Start, End, Email, Subject, Body, Location,dbid, id);

            return calendarEntry;
        }

    }
}
