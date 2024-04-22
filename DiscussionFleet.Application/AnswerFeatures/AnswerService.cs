using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

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

    public async Task<bool> MarkAcceptedAsync(QuestionId questionId, AnswerId answerId)
    {
        var question = await _appUnitOfWork.QuestionRepository.GetOneAsync(
            filter: x => x.Id == questionId,
            includes: [i => i.AcceptedAnswer],
            useSplitQuery: false
        );


        if (question?.AcceptedAnswer is not null)
        {
            question.AcceptedAnswer = new AcceptedAnswer
            {
                QuestionId = question.Id.Data,
                AnswerId = answerId.Data
            };

            await _appUnitOfWork.SaveAsync();
            return true;
        }


        if (question is not null && question.AcceptedAnswer is null)
        {
            question.AcceptedAnswer = new AcceptedAnswer
            {
                QuestionId = question.Id.Data,
                AnswerId = answerId.Data
            };

            await _appUnitOfWork.SaveAsync();
            return true;
        }

        return false;
    }

    public async Task<Answer> CreateAsync(AnswerCreateRequest dto)
    {
        var ans = new Answer
        {
            Id = new AnswerId(_guidProvider.SortableGuid()),
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