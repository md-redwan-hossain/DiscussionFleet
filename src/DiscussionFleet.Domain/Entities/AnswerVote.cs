using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class AnswerVote : Timestamp
{
    public Guid VoteGiverId { get; set; }
    public Guid AnswerId { get; set; }
    public bool IsUpvote { get; set; }
}