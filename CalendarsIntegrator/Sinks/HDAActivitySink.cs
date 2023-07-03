using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Dependencies;
using Microsoft.Extensions.DependencyInjection;
using System.Data;
using Microsoft.Extensions.Logging;

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
            _logger = logger;
            hdaClient = Services.ServiceCollection.GetRequiredService<IHDAClient>();
            this.dbID = dbID;
            _logger.LogInformation("kkkkkkkk", "Prova", "12");
            _logger.LogError(AppLogEvents.Error, "Prova", "12");
            
        }

        public string getDbID
        {
            get => dbID;
            set => dbID = value;
        }



        public Task Load(ISearch search)
        {
            // done
            this.entriesTable = hdaClient.GetActivities(search.Emails, search.From, search.To);

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
            }

            return Task.FromResult((IEnumerable<ICalendarEntry>)entries);
        }


        public Task<bool> Exists(ICalendarEntry entry)
        {
            // done
            string filterExpression = $"DataInizio = '{entry.Start}' AND DataFine = '{entry.End}' AND EMail = '{entry.Email}' AND Subject = '{entry.Subject.Replace("'", "''")}'";

            DataRow[] matchingRows = entriesTable.Select(filterExpression);

            return Task.FromResult(matchingRows.Length > 0);
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