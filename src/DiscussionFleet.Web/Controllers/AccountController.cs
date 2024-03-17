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
        // var model = _scope.Resolve<RegistrationViewModel>();
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Registration(RegistrationViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            viewModel.Resolve(_scope);
            viewModel.Act();
        }

        return View(viewModel);
    }
}