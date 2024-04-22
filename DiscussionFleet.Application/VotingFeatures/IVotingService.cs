using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Application.VotingFeatures;

public interface IVotingService
{
    Task<bool> MemberReputationUpvoteAsync(MemberId id, int positivePoint);
    Task<bool> MemberReputationDownVoteAsync(MemberId id, int negativePoint);
    Task<bool> QuestionUpvoteAsync(QuestionId questionId, MemberId voterId);
    Task<bool> QuestionDownVoteAsync(QuestionId questionId, MemberId voterId);
    Task<bool> AnswerUpvoteAsync(AnswerId answerId, MemberId voterId);
    Task<bool> AnswerDownVoteAsync(AnswerId answerId, MemberId voterId);

    Task<(bool upvote, bool downVote)> CheckQuestionVotingAbilityAsync(string? currentUserId,
        MemberId memberId, QuestionId questionId);

    Task<(bool upvote, bool downVote)> CheckAnswerVotingAbilityAsync(string? currentUserId,
        MemberId memberId, AnswerId answerId);
}