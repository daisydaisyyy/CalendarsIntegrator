using CalendarsIntegrator.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace CalendarsIntegrator.Core.Concretes
{
    public class CalendarEntry : ICalendarEntry
    {
        private DateTime _Start;
        private DateTime _End;
        private string _Email;
        private string _Subject;
        private string _Body;
        private string _Location;

        public CalendarEntry(DateTime start, DateTime end, string email, string subject, string body, string location)
        {
            _Start = start;
            _End = end;
            _Email = email;
            _Subject = subject;
            _Body = body;
            _Location = location;
            
        }

        public DateTime Start // This is your property
        {
            get => _Start;
            set => _Start = value;
        }
        public DateTime End // This is your property
        {
            get => _End;
            set => _End = value;
        }
        public string Email {
            get => _Email;
            set => _Email = value;
        }
        public string Subject
        {
            get => _Subject;
            set => _Subject = value;
        }

        public string Body
        {
            get => _Body;
            set => _Body = value;
        }

        public string Location
        {
            get => _Location;
            set => _Location = value;
        }



    }
}
