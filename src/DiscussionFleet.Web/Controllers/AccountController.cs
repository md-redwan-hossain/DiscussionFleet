using Autofac;
using DiscussionFleet.Infrastructure.Identity;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly ApplicationSignInManager _signInManager;

    public AccountController(ILifetimeScope scope,
        ApplicationSignInManager signInManager)
    {
        _scope = scope;
        _signInManager = signInManager;
    }

    [HttpGet]
    public IActionResult Registration()
    {
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


    [HttpGet]
    public async Task<IActionResult> Login()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> ForgotPassword()
    {
        return View();
    }


    [HttpPost]
    public async Task<IActionResult> LogOut(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();

        if (returnUrl != null) return LocalRedirect(returnUrl);

        return RedirectToAction(nameof(LogOut));
    }


    [HttpPost]
    public async Task<IActionResult> Profile()
    {
        return View();
    }
}