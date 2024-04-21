using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class  QuestionTag : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid TagId { get; set; }
}