using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class Question : Entity<Guid>
{
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsAnswered { get; set; }
    public long ViewCount { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public long AuthorId { get; set; }
    public AcceptedAnswer? AcceptedAnswer { get; set; }
    public ICollection<QuestionTag> Tags { get; set; }
    public ICollection<QuestionVote> Votes { get; set; }
    public ICollection<QuestionComment> Comments { get; set; }
}