using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Application.QuestionFeatures.Services;

public class QuestionService : IQuestionService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public QuestionService(IApplicationUnitOfWork appUnitOfWork, IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _appUnitOfWork = appUnitOfWork;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
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

    public async Task<Question> GetManyAsync(QuestionFilterRequest dto)
    {
      await  _appUnitOfWork.QuestionRepository.GetAllAsync(
            includes: [x=>x.AcceptedAnswer]
            );
        throw new NotImplementedException();
    }



}