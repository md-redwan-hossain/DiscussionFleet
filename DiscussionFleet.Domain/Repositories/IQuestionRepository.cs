using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Domain.Repositories;

public interface IQuestionRepository : IRepositoryBase<Question, QuestionId>
{
    Task<PagedData<Question>> GetQuestions(QuestionSortCriteria sortBy, QuestionFilterCriteria filterBy,
        DataSortOrder sortOrder, int page, int limit, ICollection<TagId> tags, string? searchText = null,
        MemberId? authorId = null);
}