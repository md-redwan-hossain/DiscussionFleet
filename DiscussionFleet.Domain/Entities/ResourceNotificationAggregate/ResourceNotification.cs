using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;

public class ResourceNotification : Entity<ResourceNotificationId>
{
    public Guid ConsumerId { get; set; }
    public ResourceNotificationType NotificationType { get; set; }
    public Guid QuestionId { get; set; }
    public string Title { get; set; }
}
// example:
// New Answer in How does comments ordering work?
// New Comment in How does comments ordering work?