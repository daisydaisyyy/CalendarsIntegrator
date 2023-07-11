using CalendarsIntegrator.Core.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator
{
    public class DefaultSearch : ISearch
    {
        public DateTime From { get; init; }
        public DateTime To { get; init; }
        public required IEnumerable<string> Emails { get; init; }
    }
}
