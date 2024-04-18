using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Domain.Entities.AnswerAggregate;

namespace DiscussionFleet.Application.AnswerFeatures;

public class AnswerService : IAnswerService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public AnswerService(IApplicationUnitOfWork appUnitOfWork, IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _appUnitOfWork = appUnitOfWork;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
    }

    public async Task<Answer> CreateAsync(AnswerCreateRequest dto)
    {
        var ans = new Answer
        {
            Id = _guidProvider.SortableGuid(),
            Body = dto.Body,
            AnswerGiverId = dto.AnswerGiverId,
            QuestionId = dto.QuestionId
        };

        ans.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.AnswerRepository.CreateAsync(ans);
        await _appUnitOfWork.SaveAsync();

        return ans;
    }
}