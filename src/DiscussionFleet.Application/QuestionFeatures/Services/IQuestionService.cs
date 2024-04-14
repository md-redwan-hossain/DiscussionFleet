using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.QuestionAggregate;

namespace DiscussionFleet.Application.QuestionFeatures.Services;

public interface IQuestionService
{
    Task CreateAsync(QuestionCreateRequest dto);
    Task<Question> GetManyAsync(QuestionFilterRequest dto);
}