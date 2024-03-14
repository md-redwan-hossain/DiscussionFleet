using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class QuestionTag : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid TagId { get; set; }
}