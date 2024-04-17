using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.AnswerAggregate;

public class AnswerComment : Timestamp
{
    public Guid AnswerId { get; set; }
    public Guid CommentId { get; set; }
}