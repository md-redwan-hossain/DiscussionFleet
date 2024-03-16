using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DiscussionFleet.Web.Models;

namespace DiscussionFleet.Web.Controllers;

public class HomePageController : Controller
{
    private readonly ILogger<HomePageController> _logger;

    public HomePageController(ILogger<HomePageController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}