namespace DiscussionFleet.Application.Common.Providers;

public interface IGuidProvider
{
    Guid SortableGuid();
    Guid RandomGuid();
}