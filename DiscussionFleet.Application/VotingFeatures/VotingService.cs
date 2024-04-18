using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using Microsoft.Extensions.Options;

namespace DiscussionFleet.Application.VotingFeatures;

public class VotingService : IVotingService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;
    private readonly ForumRulesOptions _forumRulesOptions;

    public VotingService(IApplicationUnitOfWork appUnitOfWork, IDateTimeProvider dateTimeProvider,
        IGuidProvider guidProvider, IOptions<ForumRulesOptions> forumRulesOptions)
    {
        _appUnitOfWork = appUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _forumRulesOptions = forumRulesOptions.Value;
    }

    public async Task<bool> MemberReputationUpvoteAsync(Guid id, int positivePoint)
    {
        var result = await _appUnitOfWork.MemberRepository.ReputationUpvoteAsync(id, positivePoint);

        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> MemberReputationDownVoteAsync(Guid id, int negativePoint)
    {
        var result = await _appUnitOfWork.MemberRepository.ReputationDownVoteAsync(id, negativePoint);

        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> QuestionUpvoteAsync(Guid questionId, Guid voterId)
    {
        var existingQuestionVote = await _appUnitOfWork.QuestionVoteRepository.GetOneAsync(
            filter: x => x.QuestionId == questionId && x.VoteGiverId == voterId,
            subsetSelector: s => s.Id,
            useSplitQuery: false
        );

        if (existingQuestionVote != default) return false;


        var entity = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.Upvote();
        if (result is false) return false;

        var qv = new QuestionVote
        {
            Id = _guidProvider.SortableGuid(),
            QuestionId = questionId,
            VoteGiverId = voterId
        };

        qv.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.QuestionVoteRepository.CreateAsync(qv);

        await _appUnitOfWork.MemberRepository.ReputationUpvoteAsync(entity.AuthorId,
            _forumRulesOptions.QuestionVotedUp);

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> QuestionDownVoteAsync(Guid questionId, Guid voterId)
    {
        var entity = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.DownVote();
        if (result is false) return false;

        var qv = await _appUnitOfWork.QuestionVoteRepository.GetOneAsync(
            filter: x => x.QuestionId == questionId && x.VoteGiverId == voterId,
            useSplitQuery: false
        );

        if (qv is null) return false;

        await _appUnitOfWork.QuestionVoteRepository.RemoveAsync(qv);

        await _appUnitOfWork.MemberRepository.ReputationDownVoteAsync(entity.AuthorId,
            _forumRulesOptions.QuestionVotedDown);

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> AnswerUpvoteAsync(Guid id)
    {
        var entity = await _appUnitOfWork.AnswerRepository.GetOneAsync(
            filter: x => x.Id == id,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.Upvote();
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> AnswerDownVoteAsync(Guid id)
    {
        var entity = await _appUnitOfWork.AnswerRepository.GetOneAsync(
            filter: x => x.Id == id,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.DownVote();
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }


    public async Task<bool> CanUpvoteAsync(string? currentUserId, Guid memberId, Guid questionId)
    {
        var result = await CheckVotingAbilityAsync(currentUserId, memberId, questionId);
        return result.upvote;
    }

    public async Task<bool> CanDownVoteAsync(string? currentUserId, Guid memberId, Guid questionId)
    {
        var result = await CheckVotingAbilityAsync(currentUserId, memberId, questionId);
        return result.downVote;
    }


    public async Task<(bool upvote, bool downVote)> CheckVotingAbilityAsync(string? currentUserId,
        Guid memberId, Guid questionId)
    {
        if (currentUserId is null)
        {
            return (false, false);
        }

        var parsedUserId = Guid.Parse(currentUserId);

        if (parsedUserId == memberId)
        {
            return (false, false);
        }

        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(
            filter: x => x.Id == parsedUserId,
            useSplitQuery: false
        );

        if (member is null)
        {
            return (false, false);
        }

        if (member.ReputationCount < _forumRulesOptions.MinimumReputationForVote)
        {
            return (false, false);
        }

        var voteId = await _appUnitOfWork.QuestionVoteRepository.GetOneAsync(
            filter: x => x.QuestionId == questionId && x.VoteGiverId == member.Id,
            subsetSelector: s => s.Id,
            useSplitQuery: false
        );


        if (voteId == default)
        {
            return (true, false);
        }

        if (voteId != default)
        {
            return (false, true);
        }


        return (false, false);
    }
}