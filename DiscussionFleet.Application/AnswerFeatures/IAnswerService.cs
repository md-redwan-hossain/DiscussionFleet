using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Application.AnswerFeatures;

public interface IAnswerService
{
    Task<Answer> CreateAsync(AnswerCreateRequest dto);
    Task<bool> MarkAcceptedAsync(QuestionId questionId, AnswerId answerId);

}