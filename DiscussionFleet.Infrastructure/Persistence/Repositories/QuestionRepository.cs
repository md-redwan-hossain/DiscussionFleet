using DiscussionFleet.Application.Common.Extensions;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate.Utils;
using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class QuestionRepository : Repository<Question, Guid>, IQuestionRepository
{
    public QuestionRepository(ApplicationDbContext context) : base(context)
    {
    }


    public async Task<PagedData<Question>> GetQuestions(QuestionSortCriteria sortBy,
        QuestionFilterCriteria filterBy, DataSortOrder sortOrder,
        int page, int limit, ICollection<Guid> tags)
    {
        var query = EntityDbSet
            .Include(q => q.Tags)
            .AsQueryable();

        if (filterBy.NoAnswer)
        {
            query = query.Where(q => q.IsAnswered);
        }


        if (filterBy.NoAcceptedAnswer)
        {
            query = query.Where(q => q.HasAcceptedAnswer == false);
        }

        if (tags.Count is not 0)
        {
            query = query.Where(q => q.Tags.Any(t => tags.Contains(t.TagId)));
        }

        query = sortBy switch
        {
            QuestionSortCriteria.Newest => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.CreatedAtUtc)
                : query.OrderByDescending(q => q.CreatedAtUtc),
            QuestionSortCriteria.RecentActivity => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.UpdatedAtUtc ?? q.CreatedAtUtc)
                : query.OrderByDescending(q => q.UpdatedAtUtc ?? q.CreatedAtUtc),
            QuestionSortCriteria.HighestScore => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.VoteCount)
                : query.OrderByDescending(q => q.VoteCount),
            _ => query
        };

        var totalCount = await query.CountAsync();
        query = query.Paginate(page, limit);

        return new PagedData<Question>(await query.ToListAsync(), totalCount);
    }
}