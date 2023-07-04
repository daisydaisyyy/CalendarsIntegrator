using CalendarsIntegrator;
using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Sinks;
using Microsoft.Extensions.Logging;
using Serilog.Core;

ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
{
    string currentDirectory = Directory.GetCurrentDirectory();
    string logFilePath = Path.Combine(Directory.GetParent(currentDirectory).Parent.Parent.Parent.FullName,"Logs\\logs.log");
    builder.AddFile(logFilePath);
});

ILogger<string> logger = loggerFactory.CreateLogger<string>();
Services._logger = logger;
var hdaSink = new HDAActivitySink("HDA_10",logger);
var microsoftSink = new Microsoft365Sink(logger);
var search = new DefaultSearch()
{
    Emails = new[] { "TEST_MAIL", "ADMIN_TEST_MAIL" },
    From = new DateTime(2022, 1, 1),
    To = new DateTime(2022, 12, 31)
};

var intergrator = new Integrator(new ISink[] { hdaSink }, new ISink[] { microsoftSink }, search);
await intergrator.Sync();

Environment.Exit(0);