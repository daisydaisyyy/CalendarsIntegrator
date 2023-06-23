using CalendarsIntegrator;
using CalendarsIntegrator.Core.Abstracts;
using CalendarsIntegrator.Core.Concretes;
using CalendarsIntegrator.Sinks;

var hdaSink = new HDAActivitySink();
var microsoftSink = new Microsoft365Sink();
var search = new DefaultSearch()
{
    Emails = new[] { "TEST_MAIL", "ADMIN_TEST_MAIL" },
    From = new DateTime(2022, 1, 1),
    To = new DateTime(2022, 12, 31)
};

var intergrator = new Integrator(new ISink[] { hdaSink }, new ISink[] { microsoftSink }, search);
await intergrator.Sync();

//ConfigHandler.configuration();

Console.WriteLine("End");
Environment.Exit(0);