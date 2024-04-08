using Autofac;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Infrastructure.Utils;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;
using SharpOutcome;

namespace DiscussionFleet.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly ApplicationSignInManager _signInManager;
    private readonly ApplicationUserManager _userManager;
    private readonly IMemberService _memberService;


    public AccountController(ILifetimeScope scope, ApplicationSignInManager signInManager,
        ApplicationUserManager userManager, IMemberService memberService)
    {
        _scope = scope;
        _signInManager = signInManager;
        _userManager = userManager;
        _memberService = memberService;
    }

    #region Registration

    [HttpGet]
    public IActionResult Registration()
    {
        var viewModel = _scope.Resolve<RegistrationViewModel>();
        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Registration(RegistrationViewModel viewModel)
    {
        if (ModelState.IsValid is false)
        {
            viewModel.HasError = true;
            return View(viewModel);
        }

        viewModel.Resolve(_scope);
        var result = await viewModel.ConductRegistrationAsync();

        if (result.TryPickGoodOutcome(out var userId))
        {
            TempData.TryAdd(WebConstants.AppUserId, userId);
            return RedirectToAction(nameof(ConfirmAccount));
        }

        if (result.TryPickBadOutcome(out var error))
        {
            viewModel.HasError = true;
            if (error.Reason is not BadOutcomeTag.Unknown)
            {
                foreach (var e in error.Errors)
                {
                    ModelState.AddModelError(string.Empty, e.Value);
                }

                return View(viewModel);
            }

            ModelState.AddModelError(string.Empty, "Something went wrong!");
        }

        return View(viewModel);
    }

    #endregion

    #region Login

    [HttpGet]
    public IActionResult Login()
    {
        var viewModel = _scope.Resolve<LoginViewModel>();
        return View(viewModel);
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel viewModel)
    {
        viewModel.ReturnUrl ??= Url.Content("~/");

        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password,
                isPersistent: viewModel.RememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                return LocalRedirect(viewModel.ReturnUrl);
            }

            if (result.IsNotAllowed)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);
                TempData.TryAdd(WebConstants.AppUserId, user?.Id);
                return RedirectToAction(nameof(ConfirmAccount));
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            viewModel.HasError = true;
        }

        return View(viewModel);
    }

    #endregion

    [HttpGet]
    public IActionResult ForgotPassword()
    {
        return View();
    }


    [HttpGet]
    public IActionResult ConfirmAccount()
    {
        if (TempData.TryGetValue(WebConstants.AppUserId, out var val))
        {
            if (val is null) return RedirectToAction(nameof(Login));
            var viewModel = _scope.Resolve<ConfirmAccountViewModel>();
            viewModel.UserId = (Guid)val;
            return View(viewModel);
        }

        return RedirectToAction(nameof(Login));
    }

    #region Confirm Account

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmAccount(ConfirmAccountViewModel viewModel)
    {
        if (viewModel.UserId == default)
        {
            return RedirectToAction(nameof(Login));
        }

        viewModel.Resolve(_scope);
        var result = await viewModel.ConductConfirmationAsync(viewModel.UserId.ToString(), viewModel.Code);


        if (result.TryPickBadOutcome(out var error))
        {
            ModelState.AddModelError(string.Empty, error.Reason ?? "Something went wrong!");
            viewModel.HasError = true;
            return View(viewModel);
        }

        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region Logout

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut(string? returnUrl = null)
    {
        await _signInManager.SignOutAsync();

        if (returnUrl != null)
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    #endregion


    public IActionResult ResendAccountVerificationCode()
    {
        var viewModel = _scope.Resolve<ResendAccountVerificationCodeViewModel>();
        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> ResendAccountVerificationCode(ResendAccountVerificationCodeViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            var result = await _signInManager.PasswordSignInAsync(viewModel.Email, viewModel.Password,
                isPersistent: false, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Already verified.");
                viewModel.HasError = true;
                return View(viewModel);
            }

            if (result.IsNotAllowed)
            {
                var user = await _userManager.FindByEmailAsync(viewModel.Email);

                if (user is null)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    viewModel.HasError = true;
                    return View(viewModel);
                }

                var outcome = await _memberService.ResendEmailVerificationToken(user);

                if (outcome.TryPickBadOutcome(out var err))
                {
                    if (err.Reason is BadOutcomeTag.Rejected)
                    {
                        TempData.TryAdd(WebConstants.ResendEmailTokenErr, $"Wait until {err.NextTokenAtUtc} UTC");
                    }
                    else if (err.Reason is BadOutcomeTag.NotFound)
                    {
                        TempData.TryAdd(WebConstants.ResendEmailTokenErr, "User not found.");
                    }
                }
                
                TempData.TryAdd(WebConstants.AppUserId, user.Id);
                return RedirectToAction(nameof(ConfirmAccount));
            }

            ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            viewModel.HasError = true;
        }


        // if (TempData.TryGetValue(WebConstants.AppUserId, out var val))
        // {
        //     if (val is null) return RedirectToAction(nameof(Login));
        //
        //     var userId = (Guid)val;
        //
        //     var result = await _memberService.ResendEmailVerificationToken(userId.ToString());
        //
        //     if (result.TryPickBadOutcome(out var err))
        //     {
        //         if (err.Reason is BadOutcomeTag.Rejected)
        //         {
        //             TempData.TryAdd(WebConstants.ResendEmailTokenErr, $"Wait until {err.NextTokenAtUtc} UTC");
        //         }
        //         else if (err.Reason is BadOutcomeTag.NotFound)
        //         {
        //             TempData.TryAdd(WebConstants.ResendEmailTokenErr, "User not found.");
        //         }
        //     }
        //
        //     TempData.TryAdd(WebConstants.AppUserId, val);
        //     return RedirectToAction(nameof(ConfirmAccount));
        // }

        return RedirectToAction(nameof(Login));
    }


    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(ProfileViewModel viewModel, Guid id)
    {
        var result = await viewModel.FetchMemberData(id);

        if (result.TryPickGoodOutcome(out var member))
        {
        }

        return View();
    }
}