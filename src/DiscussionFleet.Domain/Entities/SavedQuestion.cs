using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class SavedQuestion : Timestamp
{
    public Guid QuestionId { get; set; }
    public Guid UserId { get; set; }
}