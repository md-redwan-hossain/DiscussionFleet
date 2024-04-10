using DiscussionFleet.Application.MembershipFeatures;

namespace DiscussionFleet.Infrastructure.Identity;

public record ResendEmailError(ResendEmailErrorReason Reason, DateTime? NextTokenAtUtc = default) : IResendEmailError;