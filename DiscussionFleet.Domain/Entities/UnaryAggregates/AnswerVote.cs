using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class AnswerVote : Entity<Guid>
{
    public MemberId VoteGiverId { get; set; }
    public AnswerId AnswerId { get; set; }
}