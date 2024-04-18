using System.Security.Claims;
using Autofac;
using DiscussionFleet.Web.Models.Others;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class NotificationsController : Controller
{
    private readonly ILifetimeScope _scope;

    public NotificationsController(ILifetimeScope scope)
    {
        _scope = scope;
    }

    [Authorize]
    public async Task<IActionResult> Index([FromQuery] NotificationViewModel viewModel)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (currentUserId is null)
        {
            return Unauthorized();
        }

        viewModel.Resolve(_scope);
        await viewModel.FetchData(Guid.Parse(currentUserId));
        return View(viewModel);
    }
}