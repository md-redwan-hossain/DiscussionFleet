using Autofac;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILifetimeScope _scope;

    public AccountController(ILifetimeScope scope)
    {
        _scope = scope;
    }

    [HttpGet]
    public IActionResult Registration()
    {
        var model = _scope.Resolve<RegistrationModel>();
        return View(model);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Registration(RegistrationModel model)
    {
        if (ModelState.IsValid)
        {
            model.Resolve(_scope);
            model.Act();
        }

        return View(model);
    }
}