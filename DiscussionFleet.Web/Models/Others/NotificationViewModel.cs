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
    public NotificationFilterCriteria FilterBy { get; set; } = new();
    public NotificationSortCriteria SortBy { get; set; } = NotificationSortCriteria.Newest;
    [BindNever] public Paginator Pagination { get; set; }

    public ICollection<ResourceNotification> Data { get; set; }
    

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
    }
}