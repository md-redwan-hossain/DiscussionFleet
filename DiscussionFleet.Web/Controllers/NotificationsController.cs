using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class NotificationsController : Controller
{
    [Authorize]
    public IActionResult Index()
    {
        return View();
    }
}