using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.CommentAggregate;

public class Comment : Entity<CommentId>
{
    public string Body { get; set; }
    public Guid CommenterId { get; set; }
}