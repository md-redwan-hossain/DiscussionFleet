using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace DiscussionFleet.Web.ViewComponents;

public class MemberInfoForNavBar : ViewComponent
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly ApplicationUserManager _userManager;
    private readonly IMemberService _memberService;
    private readonly IFileBucketService _fileBucketService;


    public MemberInfoForNavBar(IApplicationUnitOfWork appUnitOfWork, ApplicationUserManager userManager,
        IMemberService memberService, IFileBucketService fileBucketService)
    {
        _appUnitOfWork = appUnitOfWork;
        _userManager = userManager;
        _memberService = memberService;
        _fileBucketService = fileBucketService;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var id = _userManager.GetUserId(UserClaimsPrincipal);
        if (id is null) return View();

        var cache = await _memberService.GetCachedMemberInfoAsync(id);

        if (cache is null)
        {
            var entity = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == Guid.Parse(id),
                subsetSelector: x => new { x.Id, x.FullName, x.ProfileImageId });

            if (entity is null) return View();

            var user = await _userManager.FindByIdAsync(id);
            if (user is null) return View();

            string? imgName = null;
            if (entity.ProfileImageId is not null)
            {
                var img = await _appUnitOfWork
                    .MultimediaImageRepository
                    .GetOneAsync(x => x.Id == entity.ProfileImageId);

                imgName = img?.ImageNameResolver();
            }

            var info = new MemberCachedInformation(entity.FullName, user.EmailConfirmed, imgName);
            await _memberService.CacheMemberInfoAsync(id, info);


            var dataFromDb = new NavbarUserInfoViewModel { Id = entity.Id, Name = entity.FullName };

            if (imgName is not null)
            {
                var url = await _fileBucketService.GetImageUrlAsync(imgName);
                dataFromDb.ProfilePictureUrl = url;
            }


            return View(dataFromDb);
        }

        var dataFromCache = new NavbarUserInfoViewModel { Id = Guid.Parse(id), Name = cache.FullName };

        if (cache.profileImage is not null)
        {
            var url = await _fileBucketService.GetImageUrlAsync(cache.profileImage);
            dataFromCache.ProfilePictureUrl = url;
        }

        return View(dataFromCache);
    }
}