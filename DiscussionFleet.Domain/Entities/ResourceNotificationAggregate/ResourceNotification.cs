using DiscussionFleet.Domain.Entities.Abstracts;
using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;

public class ResourceNotification : Entity<Guid>
{
    public Guid ConsumerId { get; set; }
    public ResourceNotificationType NotificationType { get; set; }
    public Guid QuestionId { get; set; }
    public string Title { get; set; }
}
// example:
// New Answer in How does comments ordering work?
// New Comment in How does comments ordering work?