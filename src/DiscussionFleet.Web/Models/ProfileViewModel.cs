using Autofac;
using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Enums;
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
    private IFileBucketService _fileBucketService;
    private IDateTimeProvider _dateTimeProvider;

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


    public ProfileViewModel(ILifetimeScope scope, IApplicationUnitOfWork appUnitOfWork, IMemberService memberService,
        IFileBucketService fileBucketService, IDateTimeProvider dateTimeProvider)
    {
        _scope = scope;
        _appUnitOfWork = appUnitOfWork;
        _memberService = memberService;
        _fileBucketService = fileBucketService;
        _dateTimeProvider = dateTimeProvider;
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
        return result;
    }

    public async Task UpdateMemberProfileImage(Guid id, IFormFile formFile)
    {
        var result = await _fileBucketService.UploadImageAsync(formFile.OpenReadStream(), formFile.ContentType,
            Path.GetExtension(formFile.FileName), id, ImagePurpose.UserProfile);

        if (result)
        {
            var member = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id, disableTracking: false);

            if (member?.ProfileImageId != null)
            {
                var existingImg = await _appUnitOfWork.MultimediaImageRepository.GetOneAsync(x => x.Id == id);
                if (existingImg is not null)
                {
                    existingImg.UpdatedAtUtc = _dateTimeProvider.CurrentUtcTime;
                    await _appUnitOfWork.SaveAsync();
                }
            }

            else if (member is not null && member.ProfileImageId is null)
            {
                var entityImage = new MultimediaImage
                {
                    Id = id,
                    Purpose = ImagePurpose.UserProfile,
                    FileExtension = Path.GetExtension(formFile.FileName)
                };
                entityImage.SetCreatedAt(_dateTimeProvider.CurrentUtcTime);
                await _appUnitOfWork.MultimediaImageRepository.CreateAsync(entityImage);
                member.ProfileImageId = entityImage.Id;
                await _appUnitOfWork.SaveAsync();
            }
        }
    }

    public async Task InvalidCacheAsync(string id)
    {
        await _memberService.FlushMemberInfoCacheAsync(id);
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _appUnitOfWork = _scope.Resolve<IApplicationUnitOfWork>();
        _memberService = _scope.Resolve<IMemberService>();
        _fileBucketService = _scope.Resolve<IFileBucketService>();
        _dateTimeProvider = _scope.Resolve<IDateTimeProvider>();
    }
}