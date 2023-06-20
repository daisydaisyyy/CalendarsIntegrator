using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CalendarsIntegrator.Dependencies.Concretes
{
    internal class HDAClient : IHDAClient, IDisposable
    {
        private SqlConnection _connection;

        public HDAClient()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

            builder.DataSource = "SOURCE";
            builder.UserID = "USER_ID";
            builder.Password = "YOUR_DB_PASSWORD";
            builder.InitialCatalog = "HDA_10";
            builder.Encrypt = false;

            _connection = new SqlConnection(builder.ConnectionString);
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public DataTable GetActivities(IEnumerable<string> email, DateTime? startDate, DateTime? endDate)
        {
            try
            {

                string sql = @"SELECT PAP.IDProtocollo, PAP.DataInizio, PAP.DataFine, PAP.Subject, PAP.Note, REPLACE (P.EMail, '-x#x-', '@') AS EMail
                    FROM TABPersonaleAttivitaProspetto PAP
                    INNER JOIN TABPersonale P ON PAP.IDPersonale = P.IDTecnico
                    WHERE PAP.IDCommessa = '900X'";

                if (email?.Count() > 0)
                {
                    var emailList = email.ToList();

                    for (int i = 0; i < emailList.Count(); i++)
                    {
                        var e = emailList.ElementAt(i);

                        if (e.Equals("ADMIN_TEST_MAIL")) e = "USER_MAIL1";
                        if (e.Equals("TEST_MAIL")) e = "USER_MAIL2";

                        emailList[i] = e;
                    }

                    var emailFilter = (from e in emailList select "'" + e.Replace("-x#x-", "@") + "'");
                    //sql += $" AND Email IN ({string.Join(", ", emailFilter)})";
                    sql += $" AND REPLACE(P.EMail, '-x#x-', '@') IN ({string.Join(", ", emailFilter)})";
                }

                if (startDate.HasValue)
                    sql += $" AND PAP.DataInizio >= CONVERT(DATE, '{startDate?.ToString("yyyy-MM-dd")}')";

                if (endDate.HasValue)
                    sql += $" AND PAP.DataFine <= CONVERT(DATE, '{endDate?.ToString("yyyy-MM-dd")}')";

                var result = GetDataTable(sql);

                foreach (DataRow item in result.Rows)
                {                              

                    if (item["EMail"].ToString().Equals("USER_MAIL1", StringComparison.InvariantCultureIgnoreCase)) 
                        item["EMail"] = "ADMIN_TEST_MAIL";

                    
                    if (item["EMail"].ToString().Equals("USER_MAIL2", StringComparison.InvariantCultureIgnoreCase))
                        item["EMail"] = "TEST_MAIL";
                }

                return result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private DataTable GetDataTable(string sqlQuery)
        {

            var result = new DataTable();
            using (SqlCommand command = new SqlCommand(sqlQuery, _connection))
            {

                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                {
                    adapter.Fill(result);
                }

            }

            return result;
        }


    }
}
