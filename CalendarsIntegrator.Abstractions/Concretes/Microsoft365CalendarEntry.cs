using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Core.Concretes
{
    public class Microsoft365CalendarEntry : CalendarEntry
    {
        private string _Id;
        public Microsoft365CalendarEntry(DateTime start, DateTime end, string email, string subject, string body, string location, string dbid, string id) : base(start, end, email, subject, body, location, dbid)
        {
            _Id = id;
        }

        public string Id
        {
            get => _Id;
            set => _Id = value;
        }
    }
}
