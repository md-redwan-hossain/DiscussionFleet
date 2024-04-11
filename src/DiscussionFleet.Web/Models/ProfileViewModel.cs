using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using Mapster;
using SharpOutcome;

namespace DiscussionFleet.Web.Models;

public class ProfileViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IApplicationUnitOfWork _appUnitOfWork;
    private IMemberService _memberService;

    public static readonly string EmptyValue = "none";

    #region Properties

    public int ReputationCount { get; set; }
    public string FullName { get; set; }
    public string? Location { get; set; }
    public string? Bio { get; set; }
    public string? PersonalWebsiteUrl { get; set; }
    public string? TwitterHandle { get; set; }
    public string? GitHubHandle { get; set; }
    public IFormFile? ProfileImage { get; set; }
    public Guid? ProfileImageId { get; set; }
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


    public async Task<Outcome<Member, IBadOutcome>> FetchMemberData(Guid id)
    {
        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id);
        if (member is not null) return member;
        return new BadOutcome(BadOutcomeTag.NotFound);
    }

    public async Task<MemberProfileUpdateResult> UpdateMemberData(Guid id)
    {
        var dto = await this.BuildAdapter().AdaptToTypeAsync<MemberUpdateRequest>();
        var result = await _memberService.UpdateAsync(dto, id);
        if (result is MemberProfileUpdateResult.Ok)
        {
            await _memberService.FlushMemberInfoCacheAsync(id.ToString());
        }

        return result;
    }


    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _memberService = _scope.Resolve<IMemberService>();
    }
}