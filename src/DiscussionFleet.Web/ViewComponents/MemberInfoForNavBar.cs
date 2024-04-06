using DiscussionFleet.Domain.Repositories;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.ViewComponents;

public class MemberInfoForNavBar : ViewComponent
{
    private readonly IMemberRepository _memberRepository;
    private readonly ApplicationUserManager _userManager;

    public MemberInfoForNavBar(IMemberRepository memberRepository, ApplicationUserManager userManager)
    {
        _memberRepository = memberRepository;
        _userManager = userManager;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var id = _userManager.GetUserId(UserClaimsPrincipal);
        if (id is null) return View();

        var entity = await _memberRepository.GetOneAsync(x => x.Id == Guid.Parse(id));
        if (entity is null) return View();
        
        var data = new NavbarUserInfoViewModel { Id = entity.Id, Name = entity.FullName };
        return View(data);
    }
}