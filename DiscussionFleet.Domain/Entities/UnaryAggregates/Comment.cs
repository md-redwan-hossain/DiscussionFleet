using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class Comment : Entity<Guid>
{
    public string Body { get; set; }
    public Guid CommenterId { get; set; }


}