using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;

namespace DiscussionFleet.Application.ResourceNotificationFeatures;

public interface IResourceNotificationService
{
    Task<bool> NotifyQuestionAuthorAsync(Guid questionAuthorId, Guid questionId, string title,
        ResourceNotificationType notificationType);
}