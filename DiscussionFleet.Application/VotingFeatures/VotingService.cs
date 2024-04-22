using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
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

    public async Task<bool> MemberReputationUpvoteAsync(MemberId id, int positivePoint)
    {
        var result = await _appUnitOfWork.MemberRepository.ReputationUpvoteAsync(id, positivePoint);

        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> MemberReputationDownVoteAsync(MemberId id, int negativePoint)
    {
        var result = await _appUnitOfWork.MemberRepository.ReputationDownVoteAsync(id, negativePoint);

        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> QuestionUpvoteAsync(QuestionId questionId, MemberId voterId)
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

        var questionVote = new QuestionVote
        {
            Id = _guidProvider.SortableGuid(),
            QuestionId = questionId,
            VoteGiverId = voterId
        };

        questionVote.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.QuestionVoteRepository.CreateAsync(questionVote);

        await _appUnitOfWork.MemberRepository.ReputationUpvoteAsync(entity.AuthorId,
            _forumRulesOptions.QuestionVotedUp);

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> QuestionDownVoteAsync(QuestionId questionId, MemberId voterId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            useSplitQuery: false
        );

        if (question is null) return false;

        var result = question.DownVote();
        if (result is false) return false;

        var questionVote = await _appUnitOfWork.QuestionVoteRepository.GetOneAsync(
            filter: x => x.QuestionId == questionId && x.VoteGiverId == voterId,
            useSplitQuery: false
        );

        if (questionVote is null) return false;

        await _appUnitOfWork.QuestionVoteRepository.RemoveAsync(questionVote);

        await _appUnitOfWork.MemberRepository.ReputationDownVoteAsync(question.AuthorId,
            _forumRulesOptions.QuestionVotedDown);

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> AnswerUpvoteAsync(AnswerId answerId, MemberId voterId)
    {
        var existingAnswerVote = await _appUnitOfWork.AnswerVoteRepository.GetOneAsync(
            filter: x => x.AnswerId == answerId && x.VoteGiverId == voterId,
            subsetSelector: s => s.Id,
            useSplitQuery: false
        );

        if (existingAnswerVote != default) return false;

        var entity = await _appUnitOfWork.AnswerRepository.GetOneAsync(
            filter: x => x.Id == answerId,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.Upvote();
        if (result is false) return false;

        var answerVote = new AnswerVote
        {
            Id = _guidProvider.SortableGuid(),
            AnswerId = answerId,
            VoteGiverId = voterId
        };

        answerVote.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.AnswerVoteRepository.CreateAsync(answerVote);

        await _appUnitOfWork.MemberRepository.ReputationUpvoteAsync(entity.AnswerGiverId,
            _forumRulesOptions.AnswerVotedUp);

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> AnswerDownVoteAsync(AnswerId answerId, MemberId voterId)
    {
        var answer = await _appUnitOfWork.AnswerRepository.GetOneAsync(
            filter: x => x.Id == answerId,
            useSplitQuery: false
        );

        if (answer is null) return false;

        var result = answer.DownVote();
        if (result is false) return false;

        var answerVote = await _appUnitOfWork.AnswerVoteRepository.GetOneAsync(
            filter: x => x.AnswerId == answerId && x.VoteGiverId == voterId,
            useSplitQuery: false
        );

        if (answerVote is null) return false;

        await _appUnitOfWork.AnswerVoteRepository.RemoveAsync(answerVote);

        await _appUnitOfWork.MemberRepository.ReputationDownVoteAsync(answer.AnswerGiverId,
            _forumRulesOptions.AnswerVotedDown);

        await _appUnitOfWork.SaveAsync();
        return true;
    }


    public async Task<(bool upvote, bool downVote)> CheckQuestionVotingAbilityAsync(string? currentUserId,
        MemberId memberId, QuestionId questionId)
    {
        if (currentUserId is null)
        {
            return (false, false);
        }

        var parsedUserId = Guid.Parse(currentUserId);

        if (parsedUserId == memberId.Data)
        {
            return (false, false);
        }

        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(
            filter: x => x.Id == new MemberId(parsedUserId),
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


    public async Task<(bool upvote, bool downVote)> CheckAnswerVotingAbilityAsync(string? currentUserId,
        MemberId memberId, AnswerId answerId)
    {
        if (currentUserId is null)
        {
            return (false, false);
        }

        var parsedUserId = Guid.Parse(currentUserId);

        if (parsedUserId == memberId.Data)
        {
            return (false, false);
        }

        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(
            filter: x => x.Id == new MemberId(parsedUserId),
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

        var voteId = await _appUnitOfWork.AnswerVoteRepository.GetOneAsync(
            filter: x => x.AnswerId == answerId && x.VoteGiverId == member.Id,
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