using DiscussionFleet.Domain.Common;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Domain.Entities.AnswerAggregate;

public class Answer : Entity<AnswerId>
{
    public required QuestionId QuestionId { get; set; }
    public required MemberId AnswerGiverId { get; set; }
    public required string Body { get; set; }
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public ICollection<AnswerComment> Comments { get; set; }

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