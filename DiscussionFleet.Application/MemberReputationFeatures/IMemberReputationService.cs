namespace DiscussionFleet.Application.MemberReputationFeatures;

public interface IMemberReputationService
{
    Task<bool> UpvoteAsync(Guid id, int positivePoint);
    Task<bool> DownVoteAsync(Guid id, int negativePoint);
}