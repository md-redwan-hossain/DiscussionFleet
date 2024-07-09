using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class QuestionComment : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid CommentId { get; set; }
}