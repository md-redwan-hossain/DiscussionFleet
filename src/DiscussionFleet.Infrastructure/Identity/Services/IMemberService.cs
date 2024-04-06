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
    Task CacheVerificationEmailHistoryAsync(string id, VerificationEmailHistory verificationEmailHistory);
    Task<bool> ConfirmEmailAsync(ApplicationUser applicationUser, string token);
    Task CacheMemberInfoAsync(string id, MemberCachedInformation memberInfo);
    Task<MemberCachedInformation?> GetCachedMemberInfoAsync(string id);
}