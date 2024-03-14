using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class Answer : Entity<Guid>
{
    public Guid QuestionId { get; set; }
    public Guid AnswerGiverId { get; set; }
    public string Body { get; set; }
    public int VoteCount { get; set; }
    public AcceptedAnswer? AcceptedAnswer { get; set; }
    public ICollection<AnswerComment> AnswerComments { get; set; }
}