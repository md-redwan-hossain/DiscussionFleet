using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.ViewComponents;

public class MemberInfoForNavBar : ViewComponent
{
    private readonly ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IMemberService _memberService;
    private readonly IDateTimeProvider _dateTimeProvider;

    public MemberInfoForNavBar(ApplicationUserManager userManager,
        IMemberService memberService, IDateTimeProvider dateTimeProvider,
        ApplicationSignInManager signInManager)
    {
        _userManager = userManager;
        _memberService = memberService;
        _dateTimeProvider = dateTimeProvider;
        _signInManager = signInManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var id = _userManager.GetUserId(UserClaimsPrincipal);
        if (id is null) return View();

        var cache = await _memberService.GetCachedMemberInfoAsync(id);

        if (cache is null)
        {
            var data = await _memberService.RefreshMemberInfoCacheAsync(id);
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
            await _memberService.UpdateMemberProfileUrlCacheAsync(id);
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