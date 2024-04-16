using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.MemberAggregate;

public class SavedAnswer : Timestamp
{
    public Guid AnswerId { get; set; }
    public Guid MemberId { get; set; }
}