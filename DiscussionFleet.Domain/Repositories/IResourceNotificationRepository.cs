using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate.Utils;
using DiscussionFleet.Domain.Utils;

namespace DiscussionFleet.Domain.Repositories;

public interface IResourceNotificationRepository : IRepositoryBase<ResourceNotification, ResourceNotificationId>
{
    Task<PagedData<ResourceNotification>> GetResourceNotifications(
        NotificationSortCriteria sortBy, NotificationFilterCriteria filterBy,
        DataSortOrder sortOrder, int page, int limit, Guid memberId);
}