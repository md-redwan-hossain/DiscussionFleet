using DiscussionFleet.Application.MembershipFeatures;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity;

public record ResendEmailError(BadOutcomeTag Reason, DateTime? NextTokenAtUtc = default) : IResendEmailError;