using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Identity.Managers;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public interface IMemberService
{
    Task<Outcome<(ApplicationUser applicationUser, Member member), IMembershipError>> CreateAsync(
        MemberRegistrationRequest dto);

    Task<string> IssueVerificationMailTokenAsync(ApplicationUser user);

    Task SendVerificationMailAsync(ApplicationUser applicationUser, Member member, string verificationCode);
    Task CacheEmailVerifyHistoryAsync(string id, ITokenRateLimiter rateLimiter);
    Task<bool> ConfirmEmailAsync(ApplicationUser user, string token);
    Task CacheMemberInfoAsync(string id, MemberCachedInformation memberInfo);
    Task<MemberCachedInformation?> GetCachedMemberInfoAsync(string id);
    Task<Outcome<bool, IResendEmailError>> ResendEmailVerificationToken(string id);
}