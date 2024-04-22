using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.AnswerAggregate.Shared;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Domain.Repositories;

public interface IAnswerRepository : IRepositoryBase<Answer, AnswerId>
{
    Task<PagedData<Answer>> GetAnswers(QuestionId questionId, int page, int limit, AnswerSortCriteria sortBy);
}