using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Domain.Entities.MemberAggregate;

public class SavedQuestion : Timestamp
{
    public QuestionId QuestionId { get; set; }
    public MemberId MemberId { get; set; }
}