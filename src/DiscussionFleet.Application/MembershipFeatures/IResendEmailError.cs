using SharpOutcome;

namespace DiscussionFleet.Application.MembershipFeatures;



public interface IResendEmailError
{
    ResendEmailErrorReason Reason { get; }
    DateTime? NextTokenAtUtc { get; }
}