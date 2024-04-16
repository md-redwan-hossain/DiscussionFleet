using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.AnswerAggregate;

public class Answer : Entity<Guid>
{
    public Guid QuestionId { get; set; }
    public Guid AnswerGiverId { get; set; }
    public string Body { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public ICollection<AnswerComment> Comments { get; set; }
}