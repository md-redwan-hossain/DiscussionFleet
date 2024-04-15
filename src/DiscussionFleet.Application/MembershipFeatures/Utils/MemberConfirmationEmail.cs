namespace DiscussionFleet.Application.MembershipFeatures.Utils;

public record MemberConfirmationEmail(Guid MemberId, string FullName, string Email, string VerificationCode);