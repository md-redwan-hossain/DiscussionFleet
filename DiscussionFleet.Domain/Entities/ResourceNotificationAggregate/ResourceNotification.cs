using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;

public class ResourceNotification : Entity<ResourceNotificationId>
{
    public MemberId ConsumerId { get; set; }
    public ResourceNotificationType NotificationType { get; set; }
    public QuestionId QuestionId { get; set; }
    public string Title { get; set; }
}
// example:
// New Answer in How does comments ordering work?
// New Comment in How does comments ordering work?