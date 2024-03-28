using DiscussionFleet.Domain.Entities.Abstracts;
using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Domain.Entities;

public class ResourceNotification : Entity<Guid>
{
    public Guid ConsumerId { get; set; }
    public ResourceNotificationType NotificationType { get; set; }
    public Guid DestinationId { get; set; }
    public string SourceTitle { get; set; }
    public bool IsMarkedAsRead { get; set; }
}

// New Answer in How does comments ordering work?
// New Comment in How does comments ordering work?