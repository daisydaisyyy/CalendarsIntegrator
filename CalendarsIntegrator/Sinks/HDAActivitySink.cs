using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Extensions.Logging;
using System.Threading;

namespace CalendarsIntegrator.Sinks
{
    public class HDAActivitySink : ISink
    {
        private readonly ILogger<string> _logger;
        private IHDAClient hdaClient;
        private DataTable entriesTable;
        public string dbID;
        public HDAActivitySink(string dbID, ILogger<string> logger)
        {

            this.dbID = dbID;
            _logger = logger;

            hdaClient = Services.ServiceCollection.GetRequiredService<IHDAClient>();
        }

        public string getDbID
        {
            get => dbID;
            set => dbID = value;
        }



        public Task Load(ISearch search)
        {
            // done
            try
            {
                this.entriesTable = hdaClient.GetActivities(search.Emails, search.From, search.To);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (this.entriesTable == null) return Task.CompletedTask;

            return Task.CompletedTask;
        }

        public Task<IEnumerable<ICalendarEntry>> GetEntries()
        {
            // done

            List<ICalendarEntry> entries = new List<ICalendarEntry>();

            foreach (DataRow activity in entriesTable.Rows)
            {
                entries.Add(new CalendarEntry((DateTime)activity["DataInizio"], (DateTime)activity["DataFine"], (string)activity["EMail"], (string)activity["Subject"], (string)activity["Note"], "", (string)activity["IDProtocollo"]+":"+dbID));
                _logger.LogInformation("Event read from database, details: Start: " + activity["DataInizio"] + " |End: " + activity["DataFine"] + " |Email: " + activity["EMail"] + " |Subject: " + activity["Subject"] + activity["IDProtocollo"] + ":" + dbID + "| ",AppLogEvents.Read);
            }

            return Task.FromResult((IEnumerable<ICalendarEntry>)entries);
        }


        public Task<bool> Exists(ICalendarEntry entry)
        {
            // done

            string filterExpression = $"DataInizio = '{entry.Start}' AND DataFine = '{entry.End}' AND EMail = '{entry.Email}' AND Subject = '{entry.Subject.Replace("'", "''")}'";

            try
            {
                DataRow[] matchingRows = entriesTable.Select(filterExpression);
                return Task.FromResult(matchingRows.Length > 0);
            }
            catch(Exception ex)
            {
                throw ex;

            }
            
           
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