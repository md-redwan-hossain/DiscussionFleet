using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class ForumRuleRepository : Repository<ForumRule, Guid>, IForumRuleRepository
{
    public ForumRuleRepository(ApplicationDbContext context) : base(context)
    {
    }
}