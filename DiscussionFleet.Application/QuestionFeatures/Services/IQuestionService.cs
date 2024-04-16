using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Outcomes;
using SharpOutcome;

namespace DiscussionFleet.Application.QuestionFeatures.Services;

public interface IQuestionService
{
    Task<Question> CreateAsync(QuestionCreateRequest dto);
    Task<Outcome<Question, IBadOutcome>> CreateWithNewTagsAsync(QuestionWithNewTagsCreateRequest dto);
}