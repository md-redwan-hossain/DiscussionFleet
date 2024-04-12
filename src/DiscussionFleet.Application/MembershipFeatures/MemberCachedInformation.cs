namespace DiscussionFleet.Application.MembershipFeatures;

public record MemberCachedInformation(
    string FullName,
    bool IsVerified,
    string? ProfileImageName = null,
    string? ProfileImageUrl = null,
    DateTime? ProfileImageUrlExpirationUtc = null,
    DateTime? LockedTillUtc = null);