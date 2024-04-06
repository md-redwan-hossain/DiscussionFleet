using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.ViewComponents;

public class MemberInfoForNavBar : ViewComponent
{
    private readonly IMemberRepository _memberRepository;
    private readonly ApplicationUserManager _userManager;
    private readonly IMemberService _memberService;

    public MemberInfoForNavBar(IMemberRepository memberRepository, ApplicationUserManager userManager,
        IMemberService memberService)
    {
        _memberRepository = memberRepository;
        _userManager = userManager;
        _memberService = memberService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var id = _userManager.GetUserId(UserClaimsPrincipal);
        if (id is null) return View();

        var cache = await _memberService.GetCachedMemberInfoAsync(id);

        if (cache is null)
        {
            var entity = await _memberRepository.GetOneAsync(x => x.Id == Guid.Parse(id),
                subsetSelector: x => new { x.Id, x.FullName });
            
            if (entity is null) return View();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return View();

            var info = new MemberCachedInformation(entity.FullName, user.EmailConfirmed, user.LockoutEnabled);
            await _memberService.CacheMemberInfoAsync(id, info);
            var dataFromDb = new NavbarUserInfoViewModel { Id = entity.Id, Name = entity.FullName };
            return View(dataFromDb);
        }

        var dataFromCache = new NavbarUserInfoViewModel { Id = Guid.Parse(id), Name = cache.FullName };
        return View(dataFromCache);
    }
}