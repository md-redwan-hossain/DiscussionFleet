using DiscussionFleet.Application.Common.Extensions;
using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.AnswerAggregate.Utils;
using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class AnswerRepository : Repository<Answer, Guid>, IAnswerRepository
{
    public AnswerRepository(ApplicationDbContext context) : base(context)
    {
    }


    public async Task<PagedData<Answer>> GetAnswers(Guid questionId, int page, int limit, AnswerSortCriteria sortBy)
    {
        var query = EntityDbSet
            .Where(x => x.QuestionId == questionId)
            .Include(x => x.Comments)
            .AsQueryable();

        query = sortBy switch
        {
            AnswerSortCriteria.Newest => query.OrderByDescending(q => q.CreatedAtUtc),
            AnswerSortCriteria.Oldest => query.OrderBy(q => q.CreatedAtUtc),
            AnswerSortCriteria.HighestScore => query.OrderByDescending(q => q.VoteCount),
            _ => query
        };

        var totalCount = await query.CountAsync();
        query = query.Paginate(page, limit);

        return new PagedData<Answer>(await query.ToListAsync(), totalCount);
    }
}