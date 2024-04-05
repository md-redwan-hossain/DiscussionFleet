using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class ResourceNotificationRepository : Repository<ResourceNotification, Guid>, IResourceNotificationRepository
{
    public ResourceNotificationRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}