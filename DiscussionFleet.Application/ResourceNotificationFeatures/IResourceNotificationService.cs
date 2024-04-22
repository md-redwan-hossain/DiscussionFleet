using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;

namespace DiscussionFleet.Application.ResourceNotificationFeatures;

public interface IResourceNotificationService
{
    Task<bool> NotifyQuestionAuthorAsync(MemberId questionAuthorId, QuestionId questionId, string title,
        ResourceNotificationType notificationType);
}