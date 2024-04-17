using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace DiscussionFleet.Web.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error([FromRoute] int id, [FromQuery] string? returnUrl)
    {
        ViewData["IsException"] = true;
        ViewData["ReturnUrl"] = returnUrl;
        ViewData["ErrorMessage"] = ReasonPhrases.GetReasonPhrase(id);
        ViewData["ErrorCode"] = id;
        return View("StatusCodeError");
    }


    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Error(string returnUrl)
    {
        return LocalRedirect(returnUrl);
    }
}