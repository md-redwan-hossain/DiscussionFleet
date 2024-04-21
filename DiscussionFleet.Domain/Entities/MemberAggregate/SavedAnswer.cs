using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.AnswerAggregate;

namespace DiscussionFleet.Domain.Entities.MemberAggregate;

public class SavedAnswer : Timestamp
{
    public AnswerId AnswerId { get; set; }
    public MemberId MemberId { get; set; }
}