using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class ResourceNotificationRepository : Repository<ResourceNotification, Guid>
{
    public ResourceNotificationRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}