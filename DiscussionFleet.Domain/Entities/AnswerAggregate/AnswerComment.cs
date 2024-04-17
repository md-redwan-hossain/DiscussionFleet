using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.AnswerAggregate;

public class AnswerComment : Timestamp
{
    public Guid AnswerId { get; set; }
    public Guid CommenterId { get; set; }
    public string Body { get; set; }
    public int UsefulVoteCount { get; set; }
}