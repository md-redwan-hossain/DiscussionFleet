namespace DiscussionFleet.Application.VotingFeatures;

public interface IVotingService
{
    Task<bool> MemberUpvoteAsync(Guid id, int positivePoint);
    Task<bool> MemberDownVoteAsync(Guid id, int negativePoint);

    Task<bool> QuestionUpvoteAsync(Guid id);
    Task<bool> QuestionDownVoteAsync(Guid id);
    
    Task<bool> AnswerUpvoteAsync(Guid id);
    Task<bool> AnswerDownVoteAsync(Guid id);
}