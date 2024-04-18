using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;
using DiscussionFleet.Application.MembershipFeatures.Enums;
using DiscussionFleet.Domain.Entities.Enums;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Mapster;

namespace DiscussionFleet.Web.Models.Account;

public class ProfileViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IMemberService _memberService;

    #region Properties

    public int ReputationCount { get; set; }
    public string FullName { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
    public string? TwitterHandle { get; set; }
    public string? GitHubHandle { get; set; }
    public IFormFile? ProfileImage { get; set; }
    public string? ProfileImageUrl { get; set; } = string.Empty;
    public bool HasError { get; set; }

    #endregion

    public ProfileViewModel()
    {
    }


    public ProfileViewModel(ILifetimeScope scope, IApplicationUnitOfWork appUnitOfWork, IMemberService memberService)
    {
        _scope = scope;
        _appUnitOfWork = appUnitOfWork;
        _memberService = memberService;
    }


    public async Task FetchMemberData(Guid id)
    {
        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id, useSplitQuery: false);
        if (member is not null)
        {
            await member.BuildAdapter().AdaptToAsync(this);
            var data = await _memberService.GetCachedMemberInfoAsync(id.ToString());
            ProfileImageUrl = data?.ProfileImageUrl;
        }
    }

    public async Task<MemberProfileUpdateResult> UpdateMemberData(Guid id)
    {
        var dto = await this.BuildAdapter().AdaptToTypeAsync<MemberUpdateRequest>();
        var result = await _memberService.UpdateAsync(dto, id);
        return result;
    }

    public async Task UpsertProfileImage(Guid id, IFormFile formFile)
    {
        var dto = new ImageUploadRequest(formFile.OpenReadStream(), formFile.ContentType,
            Path.GetExtension(formFile.FileName), id, ImagePurpose.UserProfile);

        await _memberService.UpsertMemberProfileImage(dto);
    }

    public async Task RemoveProfileImage(Guid id)
    {
        await _memberService.RemoveMemberProfileImage(id);
    }

    public async Task RefreshCacheAsync(string id)
    {
        await _memberService.RefreshMemberInfoCacheAsync(id);
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _memberService = _scope.Resolve<IMemberService>();
    }
}