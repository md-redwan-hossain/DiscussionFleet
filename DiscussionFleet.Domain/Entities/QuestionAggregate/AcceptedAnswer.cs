using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class AcceptedAnswer : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid AnswerId { get; set; }
}