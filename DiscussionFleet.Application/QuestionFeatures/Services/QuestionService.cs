using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Application.TagFeatures;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Outcomes;
using SharpOutcome;

namespace DiscussionFleet.Application.QuestionFeatures.Services;

public class QuestionService : IQuestionService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ITagService _tagService;

    public QuestionService(IApplicationUnitOfWork appUnitOfWork, IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider, ITagService tagService)
    {
        _appUnitOfWork = appUnitOfWork;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
        _tagService = tagService;
    }

    public async Task CreateAsync(QuestionCreateRequest dto)
    {
        var id = _guidProvider.SortableGuid();

        ICollection<QuestionTag> tags = [];
        foreach (var dtoTagId in dto.ExistingTags)
        {
            var questionTag = new QuestionTag { TagId = dtoTagId, QuestionId = id };
            questionTag.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
            tags.Add(questionTag);
        }

        var entity = new Question
        {
            Id = id,
            AuthorId = dto.AuthorId,
            Title = dto.Title,
            Body = dto.Body,
            Tags = tags,
        };

        entity.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
        await _appUnitOfWork.QuestionRepository.CreateAsync(entity);
        await _appUnitOfWork.SaveAsync();
    }

    public async Task<Outcome<Success, IBadOutcome>> CreateWithNewTagsAsync(QuestionWithNewTagsCreateRequest dto)
    {
        await using var trx = await _appUnitOfWork.BeginTransactionAsync();

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
                ICollection<Guid> tags = [..dto.ExistingTags, ..data.Select(x => x.Id)];
                var questionCreateRequest = new QuestionCreateRequest(dto.AuthorId, dto.Title, dto.Body, tags);
                await CreateAsync(questionCreateRequest);
            }
            
            await trx.CommitAsync();
            return Success.Return;
        }
        catch (Exception)
        {
            await trx.RollbackAsync();
        }

        return new BadOutcome(BadOutcomeTag.Unknown);
    }

    public async Task<Question> GetManyAsync(QuestionFilterRequest dto)
    {
        await _appUnitOfWork.QuestionRepository.GetAllAsync(
            includes: [x => x.AcceptedAnswer]
        );
        throw new NotImplementedException();
    }
}