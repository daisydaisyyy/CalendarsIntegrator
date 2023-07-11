using CalendarsIntegrator.Core.Abstracts;

namespace CalendarsIntegrator.CoreTests
{
    internal class TestSink : ISink
    {
      
        public List<ICalendarEntry> allEventsList = new List<ICalendarEntry>();
        private string sinkID;
        private List<CalendarsIntegrator.Core.Concretes.CalendarEntry> _entries = new List<Core.Concretes.CalendarEntry>();
        string ISink.sinkId { get => sinkID; set => sinkID = value; }

        public TestSink()
        {

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
            allEventsList.Add(entry);
        }

        public async Task Delete(ICalendarEntry entry)
        {
            // done

            // find event in allEventList, get the event id and delete it

            ICalendarEntry eventToDelete = allEventsList.FirstOrDefault(e =>
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
                    allEventsList.Remove(eventToDelete);
                }
            }
        }

        public async Task Load(ISearch search)
        {
            // done
            if (search.Emails.Contains("test@input.com"))
            {
                allEventsList.Clear();
                string startDate1 = "2022/01/01";
                string startDate2 = "2022/02/02";
                string startDate3 = "2022/03/03";
                string endDate1 = "2022/01/10";
                string endDate2 = "2022/02/20";
                string endDate3 = "2022/03/30";

                allEventsList.Add(new CalendarsIntegrator.Core.Concretes.CalendarEntry(Convert.ToDateTime(startDate1), Convert.ToDateTime(endDate1), "test@test.com", "TEST EVENT 1", "TEST EVENT 1 BODY", "", "TEST:TEST"));

                allEventsList.Add(new CalendarsIntegrator.Core.Concretes.CalendarEntry(Convert.ToDateTime(startDate2), Convert.ToDateTime(endDate2), "test@test.com", "TEST EVENT 2", "TEST EVENT 2 BODY", "", "TEST:TEST"));

                allEventsList.Add(new CalendarsIntegrator.Core.Concretes.CalendarEntry(Convert.ToDateTime(startDate3), Convert.ToDateTime(endDate3), "test@test.com", "TEST EVENT 3", "TEST EVENT 3 BODY", "", "TEST:TEST"));
            }

           

        }

    }

    
}
