using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class BadgeRepository : Repository<Badge, Guid>, IBadgeRepository
{
    public BadgeRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}