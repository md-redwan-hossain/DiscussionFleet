using DiscussionFleet.Domain.Entities.AnswerAggregate;

namespace DiscussionFleet.Application.AnswerFeatures;

public interface IAnswerService
{
    Task<Answer> CreateAsync(AnswerCreateRequest dto);
    Task<bool> MarkAcceptedAsync(Guid questionId, Guid answerId);

}