using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
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
        private DataTable entriesTable;

        public HDAActivitySink()
        {
            hdaClient = Services.ServiceCollection.GetRequiredService<IHDAClient>();
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
                entries.Add(new CalendarEntry((DateTime)activity["DataInizio"], (DateTime)activity["DataFine"], (string)activity["EMail"], (string)activity["Subject"], (string)activity["Note"],""));
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
