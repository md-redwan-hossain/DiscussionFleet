using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class BadgeRepository : Repository<Badge, Guid>, IBadgeRepository
{
    public BadgeRepository(ApplicationDbContext context) : base(context)
    {
    }
}