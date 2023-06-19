using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Core.Abstracts
{

    /// <summary>
    /// Represents a calendar entry
    /// </summary>
    public interface ICalendarEntry
    {

        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string Location { get; set; }

    }
}
