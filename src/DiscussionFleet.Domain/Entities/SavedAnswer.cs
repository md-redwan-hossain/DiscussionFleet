using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class SavedAnswer : Timestamp
{
    public Guid AnswerId { get; set; }
    public Guid MemberId { get; set; }
}