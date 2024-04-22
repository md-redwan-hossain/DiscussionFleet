using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;
using DiscussionFleet.Application.MembershipFeatures.Enums;
using DiscussionFleet.Application.MembershipFeatures.Interfaces;
using DiscussionFleet.Application.MembershipFeatures.Utils;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.MultimediaImageAggregate;
using DiscussionFleet.Domain.Outcomes;
using DiscussionFleet.Infrastructure.Identity.Managers;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public interface IMemberIdentityService
{
    Task<MemberProfileUpdateResult> UpdateAsync(MemberUpdateRequest dto, MemberId id);

    Task<Outcome<ApplicationUser, CredentialError>> RequestEmailConfirmationAsync(string userName, string password);

    Task CacheEmailVerifyHistoryAsync(string id, ITokenRateLimiter rateLimiter);
    Task<bool> CacheMemberInfoAsync(MemberId id, MemberCachedInformation memberInfo);
    Task<bool> FlushMemberInfoCacheAsync(string id);
    Task<MemberCachedInformation?> RefreshMemberInfoCacheAsync(MemberId id, uint ttlInMinute = 60);
    Task<bool> UpsertMemberProfileImage(ImageUploadRequest dto);
    Task<bool> RemoveMemberProfileImage(MultimediaImageId id);
    Task<bool> UpdateMemberProfileUrlCacheAsync(MemberId id, uint ttlInMinute = 60);
    Task<MemberCachedInformation?> GetCachedMemberInfoAsync(MemberId id);
    Task<Outcome<SuccessOutcome, IResendEmailError>> ResendEmailVerificationTokenAsync(string id);

    Task<bool> HasCorrectCredentialsAsync(string userName, string password);
    Task<bool> CanRequestEmailConfirmationAsync(string userName, string password);

    // identity dependency
    Task<Outcome<(ApplicationUser applicationUser, Member member), IMembershipError>> CreateAsync(
        MemberRegistrationRequest dto);

    Task<string> IssueVerificationMailTokenAsync(ApplicationUser user);
    Task<bool> ConfirmEmailAsync(ApplicationUser user, string token);
    Task<Outcome<SuccessOutcome, IResendEmailError>> ResendEmailVerificationTokenAsync(ApplicationUser user);
}