using Autofac;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly ApplicationSignInManager _signInManager;
    private readonly ApplicationUserManager _userManager;
    private readonly IMemberService _memberService;
    private readonly IEmailService _emailService;

    public AccountController(ILifetimeScope scope, ApplicationSignInManager signInManager,
        IMemberService memberService, IEmailService emailService,
        ApplicationUserManager userManager)
    {
        _scope = scope;
        _signInManager = signInManager;
        _memberService = memberService;
        _emailService = emailService;
        _userManager = userManager;
    }

    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(RegistrationViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        viewModel.Resolve(_scope);
        var response = await viewModel.ConductRegistrationAsync();

        if (response is not null)
        {
            foreach (var e in response.Errors)
            {
                ModelState.AddModelError(e.Key, e.Value);
            }
        }
        else
        {
            return RedirectToAction(nameof(ConfirmAccount));
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


    [HttpGet]
    public async Task<IActionResult> ConfirmAccount()
    {
        var viewModel = _scope.Resolve<ConfirmAccountViewModel>();
        return View(viewModel);
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmAccount(ConfirmAccountViewModel viewModel)
    {
        var id = _userManager.GetUserId(HttpContext.User);

        if (id is null)
        {
            return RedirectToAction(nameof(Login));
        }

        await viewModel.ConductConfirmationAsync(id, viewModel.Code);
        return RedirectToAction(nameof(HomeController.Index), nameof(HomeController).Replace("Controller", ""));
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