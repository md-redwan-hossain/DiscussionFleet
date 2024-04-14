using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.MemberAggregate;

public class SavedQuestion : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid MemberId { get; set; }
}