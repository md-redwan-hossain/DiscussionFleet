using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class QuestionComment : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid CommenterId { get; set; }
    public string Body { get; set; }
    public int UsefulVoteCount { get; set; }
}