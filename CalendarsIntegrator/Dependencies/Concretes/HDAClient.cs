using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Sinks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using System.Data;

namespace CalendarsIntegrator.Dependencies.Concretes
{
    internal class HDAClient : IHDAClient, IDisposable
    {
        private SqlConnection _connection;
        private string DbID;
        private readonly ILogger<string> _logger;

        public HDAClient(string dataSource, string userID, string password, string initialCatalog, bool encrypt)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();


            _logger = Services._logger;
            builder.DataSource = dataSource;
            builder.UserID = userID;
            builder.Password = password;
            builder.InitialCatalog = initialCatalog;
            builder.Encrypt = encrypt;
            DbID = initialCatalog;

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
                    List<string> emailList = email.ToList();

                    for (int i = 0; i < emailList.Count(); i++)
                    {
                        string e = emailList.ElementAt(i);

                        if (e.Equals("ADMIN_TEST_MAIL", StringComparison.InvariantCultureIgnoreCase)) e = "USER_MAIL1";
                        if (e.Equals("TEST_MAIL", StringComparison.InvariantCultureIgnoreCase)) e = "USER_MAIL2";

                        emailList[i] = e;
                    }

                    var emailFilter = (from e in emailList select "'" + e.Replace("-x#x-", "@") + "'");
                    sql += $" AND REPLACE(P.EMail, '-x#x-', '@') IN ({string.Join(", ", emailFilter)})";
                }

                if (startDate.HasValue)
                    sql += $" AND PAP.DataInizio >= CONVERT(DATE, '{startDate?.ToString("yyyy-MM-dd")}')";

                if (endDate.HasValue)
                    sql += $" AND PAP.DataFine <= CONVERT(DATE, '{endDate?.ToString("yyyy-MM-dd")}')";

                DataTable result = GetDataTable(sql);

                foreach (DataRow item in result.Rows)
                {                              

                    if (item["EMail"].ToString().Equals("USER_MAIL1", StringComparison.InvariantCultureIgnoreCase)) 
                        item["EMail"] = "ADMIN_TEST_MAIL";

                    
                    if (item["EMail"].ToString().Equals("USER_MAIL2", StringComparison.InvariantCultureIgnoreCase))
                        item["EMail"] = "TEST_MAIL";

                   
                }

                return result;
            }
            catch (Exception ex)
                {
                    _logger.LogError("Error reading the database, details: " + ex.StackTrace, AppLogEvents.NotRead);
                throw ex;
            }
        }

        private DataTable GetDataTable(string sqlQuery)
        {
            DataTable result = null;
            try {
                result = new DataTable();
                using (SqlCommand command = new SqlCommand(sqlQuery, _connection))
                {

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        adapter.Fill(result);
                    }

                }
                
            }
            catch(Microsoft.Data.SqlClient.SqlException e)
            {
                _logger.LogError("The load method from the database generated an exception due to an authentication error, check auth keys on configurationFile.json", AppLogEvents.Error);
                throw e;
            }

            catch (Exception e)

            {
                _logger.LogError("The load method from the database generated an exception, details: " + e.StackTrace, AppLogEvents.Error);
                throw e;
            }


            return result;

        }
        

    }
}
