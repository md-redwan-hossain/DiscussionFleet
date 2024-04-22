using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Application.TagFeatures;
using DiscussionFleet.Application.VotingFeatures;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.TagAggregate;
using Microsoft.Extensions.Options;
using SharpOutcome;

namespace DiscussionFleet.Application.QuestionFeatures.Services;

public class QuestionService : IQuestionService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITagService _tagService;
    private readonly ForumRulesOptions _forumRulesOptions;
    private readonly IVotingService _votingService;

    public QuestionService(IApplicationUnitOfWork appUnitOfWork, IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider, IOptions<ForumRulesOptions> forumRulesOptions,
        IVotingService votingService, ITagService tagService)
    {
        _appUnitOfWork = appUnitOfWork;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
        _tagService = tagService;
        _forumRulesOptions = forumRulesOptions.Value;
        _votingService = votingService;
    }

    public async Task<Question> CreateAsync(QuestionCreateRequest dto)
    {
        var id = _guidProvider.SortableGuid();

        ICollection<QuestionTag> tags = [];
        foreach (var dtoTagId in dto.ExistingTags)
        {
            var questionTag = new QuestionTag { TagId = dtoTagId, QuestionId = new QuestionId(id) };
            questionTag.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
            tags.Add(questionTag);
        }

        var entity = new Question
        {
            Id = new QuestionId(id),
            AuthorId = dto.AuthorId,
            Title = dto.Title,
            Body = dto.Body,
            Tags = tags,
        };

        entity.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
        await _appUnitOfWork.QuestionRepository.CreateAsync(entity);
        await _appUnitOfWork.SaveAsync();
        return entity;
    }

    public async Task<Outcome<Question, IBadOutcome>> CreateWithNewTagsAsync(QuestionWithNewTagsCreateRequest dto)
    {
        await using (var trx = await _appUnitOfWork.BeginTransactionAsync())
        {
            try
            {
                var outcome = await _tagService.CreateMany(dto.NewTagDto);

                if (outcome.TryPickBadOutcome(out var err))
                {
                    await trx.RollbackAsync();
                    return new BadOutcome(BadOutcomeTag.Duplicate, Helpers.DelimitedCollection(err.DuplicateData));
                }


                if (outcome.TryPickGoodOutcome(out var data))
                {
                    ICollection<TagId> tags = [..dto.ExistingTags, ..data.Select(x => x.Id)];
                    var questionCreateRequest = new QuestionCreateRequest(dto.AuthorId, dto.Title, dto.Body, tags);
                    var createdQuestion = await CreateAsync(questionCreateRequest);

                    await _votingService.MemberReputationUpvoteAsync(dto.AuthorId, _forumRulesOptions.NewQuestion);
                    await trx.CommitAsync();
                    return createdQuestion;
                }

                await trx.RollbackAsync();
                return new BadOutcome(BadOutcomeTag.Unknown);
            }
            catch (Exception)
            {
                await trx.RollbackAsync();
            }
        }


        return new BadOutcome(BadOutcomeTag.Unknown);
    }
}