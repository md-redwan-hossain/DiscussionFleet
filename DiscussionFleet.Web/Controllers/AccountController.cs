using System.Security.Claims;
using AspNetCore.ReCaptcha;
using Autofac;
using DiscussionFleet.Application.MembershipFeatures.Enums;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Utils;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharpOutcome;

namespace DiscussionFleet.Web.Controllers;

public class AccountController : Controller
{
    private readonly ILifetimeScope _scope;
    private readonly ApplicationSignInManager _signInManager;
    private readonly ApplicationUserManager _userManager;


    public AccountController(ILifetimeScope scope, ApplicationSignInManager signInManager,
        ApplicationUserManager userManager)
    {
        _scope = scope;
        _signInManager = signInManager;
        _userManager = userManager;
    }

    #region Registration

    [HttpGet]
    public IActionResult Registration()
    {
        var viewModel = _scope.Resolve<RegistrationViewModel>();
        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken, ValidateReCaptcha]
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
    public async Task<IActionResult> Login(string? returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");
        await _signInManager.SignOutAsync();

        var viewModel = _scope.Resolve<LoginViewModel>();
        viewModel.ReturnUrl = returnUrl;
        return View(viewModel);
    }


    [HttpPost, ValidateAntiForgeryToken, ValidateReCaptcha]
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


    #region Confirm Account

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


    [HttpPost, ValidateAntiForgeryToken, ValidateReCaptcha]
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

        if (result.IsGoodOutcome)
        {
            TempData.TryAdd(WebConstants.AccountConfirmed, "Confirmation successful. Please login to continue");
            return RedirectToAction(nameof(Login));
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

    [HttpGet]
    public IActionResult ResendVerificationCode()
    {
        var viewModel = _scope.Resolve<ResendVerificationCodeViewModel>();
        return View(viewModel);
    }

    [HttpPost, ValidateAntiForgeryToken, ValidateReCaptcha]
    public async Task<IActionResult> ResendVerificationCode(ResendVerificationCodeViewModel viewModel)
    {
        if (ModelState.IsValid is false) return View(viewModel);

        viewModel.Resolve(_scope);

        var result = await viewModel.ConductResendVerificationCode();

        return result.Match<IActionResult>(
            userId =>
            {
                TempData.TryAdd(WebConstants.AppUserId, userId);
                return RedirectToAction(nameof(ConfirmAccount));
            },
            errMessage =>
            {
                ModelState.AddModelError(string.Empty, errMessage);
                viewModel.HasError = true;
                return View(viewModel);
            });
    }


    [HttpGet, Authorize]
    public async Task<IActionResult> Profile()
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var viewModel = _scope.Resolve<ProfileViewModel>();

        await viewModel.FetchMemberData(Guid.Parse(currentUserId));

        return View(viewModel);
    }


    [HttpPost, ValidateAntiForgeryToken, Authorize]
    public async Task<IActionResult> Profile(ProfileViewModel viewModel)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (currentUserId is null)
        {
            return Unauthorized();
        }

        var parsedUserId = Guid.Parse(currentUserId);

        if (ModelState.IsValid is false)
        {
            viewModel.HasError = true;
            return View(viewModel);
        }

        viewModel.Resolve(_scope);

        var result = await viewModel.UpdateMemberData(parsedUserId);

        if (result is not MemberProfileUpdateResult.Ok)
        {
            ModelState.AddModelError(string.Empty, "Profile not found");
            viewModel.HasError = true;
            return View(viewModel);
        }

        if (viewModel.ProfileImage is not null)
        {
            await viewModel.UpsertProfileImage(parsedUserId, viewModel.ProfileImage);
        }

        if (viewModel.ProfileImage is null)
        {
            await viewModel.RemoveProfileImage(parsedUserId);
        }

        await viewModel.RefreshCacheAsync(currentUserId);
        return RedirectToAction(nameof(Profile));
    }
}