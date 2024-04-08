using System.Diagnostics.CodeAnalysis;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Outcomes;
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
    Task<Outcome<Success, IResendEmailError>> ResendEmailVerificationTokenAsync(string id);
    Task<Outcome<Success, IResendEmailError>> ResendEmailVerificationTokenAsync(ApplicationUser user);
    Task<bool> HasCorrectCredentialsAsync(string userName, string password);
    Task<bool> CanRequestEmailConfirmationAsync(string userName, string password);

    Task<ApplicationUser?> RequestEmailConfirmationAsync(string userName, string password);
}