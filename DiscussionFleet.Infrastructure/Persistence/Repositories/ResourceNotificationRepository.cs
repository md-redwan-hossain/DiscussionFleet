using DiscussionFleet.Application.Common.Extensions;
using DiscussionFleet.Domain.Entities.Enums;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate.Utils;
using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Domain.Utils;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class ResourceNotificationRepository : Repository<ResourceNotification, Guid>, IResourceNotificationRepository
{
    public ResourceNotificationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<PagedData<ResourceNotification>> GetResourceNotifications(
        NotificationSortCriteria sortBy, NotificationFilterCriteria filterBy,
        DataSortOrder sortOrder, int page, int limit, Guid memberId)
    {
        var query = EntityDbSet.Where(q => q.ConsumerId == memberId);

        if (filterBy.Both)
        {
            query = query.Where(q =>
                q.NotificationType == ResourceNotificationType.Answer ||
                q.NotificationType == ResourceNotificationType.Comment);
        }

        if (filterBy.AnswerOnly)
        {
            query = query.Where(q => q.NotificationType == ResourceNotificationType.Answer);
        }


        if (filterBy.CommentOnly)
        {
            query = query.Where(q => q.NotificationType == ResourceNotificationType.Comment);
        }


        query = sortBy switch
        {
            NotificationSortCriteria.Newest => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.CreatedAtUtc)
                : query.OrderByDescending(q => q.CreatedAtUtc),
            NotificationSortCriteria.Oldest => sortOrder == DataSortOrder.Asc
                ? query.OrderBy(q => q.UpdatedAtUtc ?? q.CreatedAtUtc)
                : query.OrderByDescending(q => q.UpdatedAtUtc ?? q.CreatedAtUtc),
            _ => query
        };

        var totalCount = await query.CountAsync();
        query = query.Paginate(page, limit);

        return new PagedData<ResourceNotification>(await query.ToListAsync(), totalCount);
    }
}