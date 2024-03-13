namespace DiscussionFleet.Application.Common.Providers;

public interface IDateTimeProvider
{
    DateTime CurrentUtcTime { get; }
    DateTime CurrentLocalTime { get; }
}