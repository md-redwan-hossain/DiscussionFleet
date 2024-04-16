using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.MemberAggregate;

public class MemberBadge : Timestamp
{
    public Guid BadgeId { get; set; }
    public Guid MemberId { get; set; }
}