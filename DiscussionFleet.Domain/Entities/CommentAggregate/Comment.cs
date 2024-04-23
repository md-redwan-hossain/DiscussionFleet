using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.MemberAggregate;

namespace DiscussionFleet.Domain.Entities.CommentAggregate;

public class Comment : Entity<CommentId>
{
    public required string Body { get; set; }
    public required MemberId CommenterId { get; set; }
}