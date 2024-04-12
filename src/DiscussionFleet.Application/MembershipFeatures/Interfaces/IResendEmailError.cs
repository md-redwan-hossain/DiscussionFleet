using DiscussionFleet.Application.MembershipFeatures.Enums;

namespace DiscussionFleet.Application.MembershipFeatures.Interfaces;

public interface IResendEmailError
{
    ResendEmailErrorReason Reason { get; }
    DateTime? NextTokenAtUtc { get; }
}