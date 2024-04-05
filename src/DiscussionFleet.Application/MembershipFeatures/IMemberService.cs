using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using SharpOutcome;

namespace DiscussionFleet.Application.MembershipFeatures;

public interface IMemberService
{
    Task<Outcome<Member, IMembershipError>> CreateAsync(MemberRegistrationRequest dto);
}