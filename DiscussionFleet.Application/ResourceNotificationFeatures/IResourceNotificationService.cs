using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Application.ResourceNotificationFeatures;

public interface IResourceNotificationService
{
    Task<bool> NotifyQuestionAuthorAsync(Guid questionAuthorId, Guid questionId, string title,
        ResourceNotificationType notificationType);
}