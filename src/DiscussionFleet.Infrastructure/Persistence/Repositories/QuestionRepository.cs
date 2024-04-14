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


    public async Task<ICollection<Question>> GetQuestions(QuestionSortCriteria sortBy, QuestionFilterCriteria filterBy,
        DataSortOrder sortOrder, int page, int limit, ICollection<Guid> tags)
    {
        // Start with a query that gets all questions
        // var query = EntityDbSet.AsQueryable();

        var query = EntityDbSet
            .Include(q => q.Tags)
            .AsQueryable();

        // Apply the NoAnswer filter if it's set
        if (filterBy.NoAnswer)
        {
            query = query.Where(q => q.IsAnswered);
        }

        // Apply the NoAcceptedAnswer filter if it's set
        if (filterBy.NoAcceptedAnswer)
        {
            query = query.Where(q => q.HasAcceptedAnswer == false);
        }

        // Apply the Tags filter if it's set
        if (tags.Count != 0)
        {
            query = query.Where(q => q.Tags.Any(t => tags.Contains(t.TagId)));
        }

        // Apply sorting
        query = sortBy switch
        {
            QuestionSortCriteria.Newest => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.CreatedAtUtc)
                : query.OrderByDescending(q => q.CreatedAtUtc),
            QuestionSortCriteria.RecentActivity => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.UpdatedAtUtc)
                : query.OrderByDescending(q => q.UpdatedAtUtc),
            QuestionSortCriteria.HighestScore => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.VoteCount)
                : query.OrderByDescending(q => q.VoteCount),
            _ => query
        };

        query = query.Paginate(page, limit);

        return await query.ToListAsync();
    }
}