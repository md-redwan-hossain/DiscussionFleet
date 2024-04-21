using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class AnswerVote : Entity<Guid>
{
    public Guid VoteGiverId { get; set; }
    public Guid AnswerId { get; set; }
}