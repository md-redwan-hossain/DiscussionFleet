using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.MemberAggregate;

namespace DiscussionFleet.Domain.Entities.QuestionAggregate;

public class Question : Entity<QuestionId>
{
    public string Title { get; set; }
    public string Body { get; set; }
    public bool IsAnswered { get; set; }
    public bool HasAcceptedAnswer { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public MemberId AuthorId { get; set; }
    public AcceptedAnswer? AcceptedAnswer { get; set; }
    public ICollection<QuestionTag> Tags { get; set; }
    public ICollection<QuestionComment> Comments { get; set; }

    public bool Upvote()
    {
        VoteCount += 1;
        return true;
    }

    public bool DownVote()
    {
        if (VoteCount == 0)
        {
            return false;
        }

        VoteCount -= 1;
        return true;
    }
}