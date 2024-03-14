using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class Answer : Entity<Guid>
{
    public long QuestionId { get; set; }
    public long AnswerGiverId { get; set; }
    public string Body { get; set; }
    public int VoteCount { get; set; }
    public AcceptedAnswer? AcceptedAnswer { get; set; }
    public ICollection<AnswerComment> AnswerComments { get; set; }
}