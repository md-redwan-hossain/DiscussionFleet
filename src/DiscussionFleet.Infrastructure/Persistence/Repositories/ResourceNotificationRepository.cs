using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class ResourceNotificationRepository : Repository<ResourceNotification, Guid>, IResourceNotificationRepository
{
    public ResourceNotificationRepository(ApplicationDbContext context) : base(context)
    {
    }
}