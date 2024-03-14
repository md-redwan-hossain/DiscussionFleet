using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class SavedQuestion : Timestamp
{
    public long QuestionId { get; set; }
    public long UserId { get; set; }
}