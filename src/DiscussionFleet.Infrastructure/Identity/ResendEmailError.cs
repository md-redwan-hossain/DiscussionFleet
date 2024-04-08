using DiscussionFleet.Application.MembershipFeatures;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity;

public record ResendEmailError(ResendEmailErrorReason Reason, DateTime? NextTokenAtUtc = default) : IResendEmailError;