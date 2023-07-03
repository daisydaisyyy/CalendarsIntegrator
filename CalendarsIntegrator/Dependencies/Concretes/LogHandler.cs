using Microsoft.Extensions.Configuration;
using System.Runtime.CompilerServices;
using System.Text.Json;

namespace CalendarsIntegrator.Core.Concretes
{
    public static class LogHandler
    {
        static string currentDirectory;
        static string parentDirectory;
        static string logDirectory;
        static string logFileName;

        static string logFilePath;

        public static void initialize()
        {
            try
            {
                currentDirectory = Directory.GetCurrentDirectory();
                parentDirectory = Directory.GetParent(Directory.GetParent(currentDirectory).FullName).FullName;
                logDirectory = parentDirectory;
                logFileName = "CalendarsIntegrator.log";

                logFilePath = Path.Combine(logDirectory, logFileName);

                WriteOnLog("\n--------------------------------------------\nEXECUTED ON DATE " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss") + "\nINFO:\n");

            }
            catch (Exception ex)
            {
                Console.WriteLine("There was an error loading the log, details: "+ex.StackTrace);
            }

        }
        

        public static void WriteOnLog(string msg)
        {
            using (StreamWriter sw = File.AppendText(logFilePath))
            {
                sw.WriteLine(msg);
            }
        }

        public static bool didGenerateExceptions = false;
    }
}