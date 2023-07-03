using Microsoft.Extensions.Logging;

internal static class AppLogEvents
{
    internal static EventId Create = new(1000, "Created");
    internal static EventId Read = new(1001, "Read");
    internal static EventId Delete = new(1002, "Deleted");
    internal static EventId Details = new(1003);


    internal static EventId NotCreated = new(2000, "NotCreated");
    internal static EventId NotRead = new(2001, "NotRead");
    internal static EventId NotDeleted = new(2002, "NotDeleted");
    internal static EventId Error = new(2004);
}