using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.MemberAggregate;

public class Member : Entity<Guid>
{
    public int ReputationCount { get; set; } = 1;
    public string FullName { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
    public string? TwitterHandle { get; set; }
    public string? GithubHandle { get; set; }
    public Guid? ProfileImageId { get; set; }
    public ICollection<SavedQuestion> SavedQuestions { get; set; }
    public ICollection<SavedAnswer> SavedAnswers { get; set; }

    public bool Upvote(int positivePoint)
    {
        if (positivePoint <= 0) return false;
    
        try
        {
            checked
            {
                ReputationCount += positivePoint;
                return true;
            }
        }
        catch (OverflowException)
        {
            return false;
        }
    }


    public bool DownVote(int negativePoint)
    {
        if (negativePoint >= 0) return false;
        if (ReputationCount == 1) return false;
        
        try
        {
            checked
            {
                var reputationCountMinusOne = ReputationCount - 1;
                var toPositivePoint = negativePoint * -1;
                var availablePointToSubtract = toPositivePoint - reputationCountMinusOne;
                var toApply = negativePoint - availablePointToSubtract;
                ReputationCount -= toApply;

                return true;
            }
        }
        catch (OverflowException)
        {
            return false;
        }
    }
}