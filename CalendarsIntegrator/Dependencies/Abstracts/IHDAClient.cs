using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Dependencies
{
    internal interface IHDAClient
    {


        public DataTable GetActivities(IEnumerable<string> email, DateTime? startDate, DateTime? endDate); 

    }
}
