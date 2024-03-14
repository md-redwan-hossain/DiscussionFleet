using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class SavedAnswer : Timestamp
{
    public long AnswerId { get; set; }
    public long UserId { get; set; }
}