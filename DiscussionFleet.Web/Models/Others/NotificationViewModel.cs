using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate.Utils;
using DiscussionFleet.Domain.Utils;
using DiscussionFleet.Web.Utils;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DiscussionFleet.Web.Models.Others;

public class NotificationViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;


    public NotificationViewModel()
    {
    }


    public NotificationViewModel(IApplicationUnitOfWork appUnitOfWork)
    {
        _appUnitOfWork = appUnitOfWork;
    }

    public byte DataPerPage { get; set; } = 15;
    public int CurrentPage { get; set; } = 1;
    public DataSortOrder SortOrder { get; set; } = DataSortOrder.Asc;
    public NotificationSortCriteria SortBy { get; set; } = NotificationSortCriteria.Newest;
    [BindNever] public Paginator Pagination { get; set; }
    public ICollection<ResourceNotification> Data { get; set; }


    public async Task FetchData(Guid userId)
    {
        var filterBy = new NotificationFilterCriteria { Both = true };

        if (DataPerPage < 15) DataPerPage = 15;
        var (questions, total) = await _appUnitOfWork.ResourceNotificationRepository.GetResourceNotifications(SortBy,
            filterBy, SortOrder, CurrentPage, DataPerPage, userId
        );

        var pager = new Paginator(totalItems: total, dataPerPage: DataPerPage, currentPage: CurrentPage);

        Pagination = pager;

        Data = questions;
    }


    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
    }
}