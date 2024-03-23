using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DiscussionFleet.Web.Models;

namespace DiscussionFleet.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
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
    public IActionResult Error(int id)
    {
        ViewData["IsException"] = true;
        ViewData["ErrorCode"] = id;
        if (id == 404) return View("NotFound");

        return View("StatusCodeError");
    }
}