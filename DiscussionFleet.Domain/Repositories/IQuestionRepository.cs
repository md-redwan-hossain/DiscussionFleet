using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Domain.Repositories;

public interface IQuestionRepository : IRepositoryBase<Question, QuestionId>
{
    Task<PagedData<Question>> GetQuestions(QuestionSortCriteria sortBy, QuestionFilterCriteria filterBy,
        DataSortOrder sortOrder, int page, int limit, ICollection<Guid> tags, string? searchText = null,
        Guid? authorId = null);
}