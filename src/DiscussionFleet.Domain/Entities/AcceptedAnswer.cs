using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class AcceptedAnswer : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid AnswerId { get; set; }
}