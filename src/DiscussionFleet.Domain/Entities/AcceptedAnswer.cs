using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class AcceptedAnswer : Timestamp
{
    public long QuestionId { get; set; }
    public long AnswerId { get; set; }
}