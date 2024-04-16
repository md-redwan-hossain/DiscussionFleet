using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;
using DiscussionFleet.Application.MembershipFeatures.Enums;
using DiscussionFleet.Application.MembershipFeatures.Interfaces;
using DiscussionFleet.Application.MembershipFeatures.Utils;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Outcomes;
using DiscussionFleet.Infrastructure.Identity.Managers;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public interface IMemberService
{
    Task<Outcome<(ApplicationUser applicationUser, Member member), IMembershipError>> CreateAsync(
        MemberRegistrationRequest dto);

    Task<MemberProfileUpdateResult> UpdateAsync(MemberUpdateRequest dto, System.Guid id);
    Task<string> IssueVerificationMailTokenAsync(ApplicationUser user);
    Task CacheEmailVerifyHistoryAsync(string id, ITokenRateLimiter rateLimiter);
    Task<bool> ConfirmEmailAsync(ApplicationUser user, string token);
    Task<bool> CacheMemberInfoAsync(string id, MemberCachedInformation memberInfo);
    Task<bool> FlushMemberInfoCacheAsync(string id);
    Task<MemberCachedInformation?> RefreshMemberInfoCacheAsync(string id, uint ttlInMinute = 60);
    Task<bool> UpsertMemberProfileImage(ImageUploadRequest dto);
    Task<bool> RemoveMemberProfileImage(System.Guid id);
    Task<bool> UpdateMemberProfileUrlCacheAsync(string id, uint ttlInMinute = 60);
    Task<MemberCachedInformation?> GetCachedMemberInfoAsync(string id);
    Task<Outcome<SuccessOutcome, IResendEmailError>> ResendEmailVerificationTokenAsync(string id);
    Task<Outcome<SuccessOutcome, IResendEmailError>> ResendEmailVerificationTokenAsync(ApplicationUser user);
    Task<bool> HasCorrectCredentialsAsync(string userName, string password);
    Task<bool> CanRequestEmailConfirmationAsync(string userName, string password);
    Task<Outcome<ApplicationUser, CredentialError>> RequestEmailConfirmationAsync(string userName, string password);
}