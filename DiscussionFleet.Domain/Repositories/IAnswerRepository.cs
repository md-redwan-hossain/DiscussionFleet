using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.AnswerAggregate.Utils;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Domain.Repositories;

public interface IAnswerRepository : IRepositoryBase<Answer, Guid>
{
    Task<PagedData<Answer>> GetAnswers(Guid questionId, int page, int limit, AnswerSortCriteria sortBy);
}