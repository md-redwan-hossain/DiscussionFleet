namespace DiscussionFleet.Domain.Entities;

public class AcceptedAnswer
{
    public Guid QuestionId { get; set; }
    public Guid AnswerId { get; set; }
}