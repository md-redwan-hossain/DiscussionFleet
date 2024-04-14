using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class ForumRule : Entity<Guid>
{
    public int MinimumReputationForVote { get; set; }
    public int QuestionVotedUp { get; set; }
    public int AnswerVotedUp { get; set; }
    public int ArticleVotedUp { get; set; }
    public int AnswerMarkedAccepted { get; set; }
    public int AnswerAcceptor { get; set; }
    public int SuggestedEditAccepted { get; set; }
    public int QuestionVotedDown { get; set; }
    public int AnswerVotedDown { get; set; }
    public int ArticleVotedDown { get; set; }
    public int PostFlagged { get; set; }
}