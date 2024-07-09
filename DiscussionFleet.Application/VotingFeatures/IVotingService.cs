namespace DiscussionFleet.Application.VotingFeatures;

public interface IVotingService
{
    Task<bool> MemberReputationUpvoteAsync(Guid id, int positivePoint);
    Task<bool> MemberReputationDownVoteAsync(Guid id, int negativePoint);
    Task<bool> QuestionUpvoteAsync(Guid questionId, Guid voterId);
    Task<bool> QuestionDownVoteAsync(Guid questionId, Guid voterId);
    Task<bool> AnswerUpvoteAsync(Guid answerId, Guid voterId);
    Task<bool> AnswerDownVoteAsync(Guid answerId, Guid voterId);

    Task<(bool upvote, bool downVote)> CheckQuestionVotingAbilityAsync(string? currentUserId,
        Guid memberId, Guid questionId);

    Task<(bool upvote, bool downVote)> CheckAnswerVotingAbilityAsync(string? currentUserId,
        Guid memberId, Guid answerId);
}