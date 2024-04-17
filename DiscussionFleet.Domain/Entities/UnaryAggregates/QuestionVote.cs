using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class QuestionVote : Entity<Guid>
{
    public Guid VoteGiverId { get; set; }
    public Guid QuestionId { get; set; }
}