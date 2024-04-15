using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Outcomes;
using SharpOutcome;

namespace DiscussionFleet.Application.QuestionFeatures.Services;

public interface IQuestionService
{
    Task CreateAsync(QuestionCreateRequest dto);
    Task<Outcome<Success, IBadOutcome>> CreateWithNewTagsAsync(QuestionWithNewTagsCreateRequest dto);
    Task<Question> GetManyAsync(QuestionFilterRequest dto);
}