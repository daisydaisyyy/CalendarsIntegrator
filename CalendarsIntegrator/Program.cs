using CalendarsIntegrator;
using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Sinks;
using Microsoft.Extensions.Logging;

LogHandler.initialize();  //2 replace


ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole();
});

ILogger<HDAActivitySink> logger = loggerFactory.CreateLogger<HDAActivitySink>();

var hdaSink = new HDAActivitySink("HDA_10",logger);
var microsoftSink = new Microsoft365Sink();
var search = new DefaultSearch()
{
    Emails = new[] { "TEST_MAIL", "ADMIN_TEST_MAIL" },
    From = new DateTime(2022, 1, 1),
    To = new DateTime(2022, 12, 31)
};

var intergrator = new Integrator(new ISink[] { hdaSink }, new ISink[] { microsoftSink }, search);
await intergrator.Sync();

if (!LogHandler.didGenerateExceptions)
    LogHandler.WriteOnLog("\nCalendar synchronized successfully.");
else
    LogHandler.WriteOnLog("\nThe program generated exceptions, the calendar was not synchronized correctly.");


Environment.Exit(0);