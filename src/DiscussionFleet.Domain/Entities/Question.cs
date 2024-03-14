using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class Question : Entity<Guid>
{
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsAnswered { get; set; }
    public long ViewCount { get; set; }
    public int VoteCount { get; set; }
    public long AuthorId { get; set; }
    public ICollection<QuestionTag> QuestionTags { get; set; }
    public ICollection<QuestionComment> QuestionComments { get; set; }
}