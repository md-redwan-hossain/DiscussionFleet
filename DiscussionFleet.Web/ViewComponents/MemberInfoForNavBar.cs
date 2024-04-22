using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Models.Others;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.ViewComponents;

public class MemberInfoForNavBar : ViewComponent
{
    private readonly ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IMemberIdentityService _memberIdentityService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MemberInfoForNavBar(ApplicationUserManager userManager,
        IMemberIdentityService memberIdentityService, IDateTimeProvider dateTimeProvider,
        ApplicationSignInManager signInManager)
    {
        _userManager = userManager;
        _memberIdentityService = memberIdentityService;
        _dateTimeProvider = dateTimeProvider;
        _signInManager = signInManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var id = _userManager.GetUserId(UserClaimsPrincipal);
        if (id is null)
        {
            await _signInManager.SignOutAsync();
            HttpContext.Response.Redirect("Account/Login/");
            return View(new NavbarUserInfoViewModel());
        }

        var cache = await _memberIdentityService.GetCachedMemberInfoAsync(id);

        if (cache is null)
        {
            var data = await _memberIdentityService.RefreshMemberInfoCacheAsync(id);
            if (data is null)
            {
                await _signInManager.SignOutAsync();
                HttpContext.Response.Redirect("Account/Login/");
                return View(new NavbarUserInfoViewModel());
            }

            cache = data;
        }

        if (IsProfileImgUrlExpired(cache.ProfileImageUrlExpirationUtc))
        {
            await _memberIdentityService.UpdateMemberProfileUrlCacheAsync(id);
        }

        var dataFromCache = new NavbarUserInfoViewModel
        {
            Id = Guid.Parse(id),
            Name = cache.FullName,
            ProfilePictureUrl = cache.ProfileImageUrl ?? string.Empty
        };

        return View(dataFromCache);
    }


    private bool IsProfileImgUrlExpired(DateTime? ttlUpperBoundUtc)
    {
        if (ttlUpperBoundUtc is null) return false;
        return _dateTimeProvider.CurrentUtcTime >= ttlUpperBoundUtc.Value;
    }
}