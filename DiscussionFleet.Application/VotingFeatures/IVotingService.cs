namespace DiscussionFleet.Application.VotingFeatures;

public interface IVotingService
{
    Task<bool> MemberReputationUpvoteAsync(Guid id, int positivePoint);
    Task<bool> MemberReputationDownVoteAsync(Guid id, int negativePoint);
    Task<bool> QuestionUpvoteAsync(Guid questionId, Guid voterId);
    Task<bool> QuestionDownVoteAsync(Guid questionId, Guid voterId);
    Task<bool> AnswerUpvoteAsync(Guid id);
    Task<bool> AnswerDownVoteAsync(Guid id);

    Task<(bool upvote, bool downVote)> CheckVotingAbilityAsync(string? currentUserId,
        Guid memberId, Guid questionId);

    Task<bool> CanUpvoteAsync(string? currentUserId, Guid memberId, Guid questionId);
    Task<bool> CanDownVoteAsync(string? currentUserId, Guid memberId, Guid questionId);
}