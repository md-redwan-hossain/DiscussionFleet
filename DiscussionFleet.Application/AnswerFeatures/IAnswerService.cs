using DiscussionFleet.Domain.Entities.AnswerAggregate;

namespace DiscussionFleet.Application.AnswerFeatures;

public interface IAnswerService
{
    Task<Answer> CreateAsync(AnswerCreateRequest dto);
    // Task<Answer> IsValidAnswerGiver(Guid dto);
}