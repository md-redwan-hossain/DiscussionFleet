using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;

namespace DiscussionFleet.Application.ResourceNotificationFeatures;

public class ResourceNotificationService : IResourceNotificationService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;


    public ResourceNotificationService(IApplicationUnitOfWork appUnitOfWork, IDateTimeProvider dateTimeProvider,
        IGuidProvider guidProvider)
    {
        _appUnitOfWork = appUnitOfWork;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
    }

    public async Task<bool> NotifyQuestionAuthorAsync(MemberId questionAuthorId, QuestionId questionId, string title,
        ResourceNotificationType notificationType)
    {
        var resourceNotification = new ResourceNotification
        {
            Id = new ResourceNotificationId(_guidProvider.SortableGuid()),
            ConsumerId = questionAuthorId,
            QuestionId = questionId,
            Title = title,
            NotificationType = notificationType
        };

        resourceNotification.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);

        await _appUnitOfWork.ResourceNotificationRepository.CreateAsync(resourceNotification);
        await _appUnitOfWork.SaveAsync();

        return true;
    }
}