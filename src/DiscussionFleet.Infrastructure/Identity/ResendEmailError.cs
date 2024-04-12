using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Application.MembershipFeatures.Enums;
using DiscussionFleet.Application.MembershipFeatures.Interfaces;

namespace DiscussionFleet.Infrastructure.Identity;

public record ResendEmailError(ResendEmailErrorReason Reason, DateTime? NextTokenAtUtc = default) : IResendEmailError;