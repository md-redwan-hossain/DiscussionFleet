using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class ForumRuleRepository : Repository<ForumRule, Guid>, IForumRuleRepository
{
    public ForumRuleRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}