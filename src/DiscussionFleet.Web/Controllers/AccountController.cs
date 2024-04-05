using Autofac;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IMemberService _memberService;

    public AccountController(ILifetimeScope scope, ApplicationSignInManager signInManager,
        IMemberService memberService)
    {
        _scope = scope;
        _signInManager = signInManager;
        _memberService = memberService;
    }

    [HttpGet]
    public IActionResult Registration()
    {
        return View();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(RegistrationViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var dto = new MemberRegistrationRequest(viewModel.FullName, viewModel.Email, viewModel.Password);
            var result = await _memberService.CreateAsync(dto);

            return result.Match(
                _ => View(),
                err =>
                {
                    foreach (var e in err.Errors)
                    {
                        ModelState.AddModelError(e.Key, e.Value);
                    }

                    return View();
                });

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