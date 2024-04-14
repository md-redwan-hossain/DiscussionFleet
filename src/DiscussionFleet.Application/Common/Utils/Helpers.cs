namespace DiscussionFleet.Application.Common.Utils;

public static class Helpers
{
    public static string DelimitedCollection<T>(IEnumerable<T> collection, string delimiter = ", ")
    {
        return string.Join(delimiter, collection);
    }
}