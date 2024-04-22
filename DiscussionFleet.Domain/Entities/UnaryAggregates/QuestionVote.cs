using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class QuestionVote : Entity<Guid>
{
    public MemberId VoteGiverId { get; set; }
    public QuestionId QuestionId { get; set; }  
}