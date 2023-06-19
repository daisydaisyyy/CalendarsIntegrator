using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Core.Abstracts
{

    /// <summary>
    /// Represents a search on calendar entries in a sink
    /// </summary>
    public interface ISearch
    {

        /// <summary>
        /// Initial date
        /// </summary>
        public DateTime From { get; init; }

        /// <summary>
        /// End date
        /// </summary>
        public DateTime To { get; init; }

        /// <summary>
        /// Operators email
        /// </summary>
        public IEnumerable<string> Emails { get; init; }



    }
}
