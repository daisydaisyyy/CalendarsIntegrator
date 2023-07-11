using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Core.Abstracts
{

    /// <summary>
    /// Represent a sink that can be the source or the destination of calendar integration
    /// </summary>
    public interface ISink
    {
        public string sinkId { get; set; }
        /// <summary>
        /// Load calendar entries
        /// </summary>
        /// <param name="search"></param>
        /// <returns></returns>
        public Task Load(ISearch search);

        /// <summary>
        /// Return calendar entries
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ICalendarEntry>> GetEntries();

        /// <summary>
        /// Assess if an entry exists
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Task<bool> Exists(ICalendarEntry entry);

        /// <summary>
        /// Insert a new entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Task Insert(ICalendarEntry entry);

        /// <summary>
        /// Delete existing entry
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public Task Delete(ICalendarEntry entry);

        

    }
}
