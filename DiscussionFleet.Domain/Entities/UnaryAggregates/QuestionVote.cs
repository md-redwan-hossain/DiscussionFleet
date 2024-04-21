using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class QuestionVote : Entity<Guid>
{
    public Guid VoteGiverId { get; set; }
    public Guid QuestionId { get; set; }
}