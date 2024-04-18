namespace DiscussionFleet.Domain.Entities.ResourceNotificationAggregate.Utils;

public class NotificationFilterCriteria
{
    public bool Both { get; set; }
    public bool AnswerOnly { get; set; }
    public bool CommentOnly { get; set; }
}