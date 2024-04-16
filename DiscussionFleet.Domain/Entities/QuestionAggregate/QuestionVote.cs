using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class QuestionVote : Timestamp
{
    public Guid VoteGiverId { get; set; }
    public Guid QuestionId { get; set; }
    public bool IsUpvote { get; set; }
}