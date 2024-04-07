using SharpOutcome;

namespace DiscussionFleet.Application.MembershipFeatures;

public interface IResendEmailError
{
    BadOutcomeTag Reason { get; }
    DateTime? NextTokenAtUtc { get; }
}