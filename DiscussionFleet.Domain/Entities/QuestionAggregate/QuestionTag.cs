using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.TagAggregate;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class  QuestionTag : Timestamp
{
    public QuestionId QuestionId { get; set; }
    public TagId TagId { get; set; }
}