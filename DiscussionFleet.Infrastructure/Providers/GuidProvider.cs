using DiscussionFleet.Application.Common.Providers;

namespace DiscussionFleet.Infrastructure.Providers;

public class GuidProvider : IGuidProvider
{
    public Guid SortableGuid() => Ulid.NewUlid().ToGuid();

    public Guid RandomGuid() => Guid.NewGuid();
}