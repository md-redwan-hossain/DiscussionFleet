namespace DiscussionFleet.Application.Common.Utils;

public record MemberCachedInformation(
    string FullName,
    bool IsVerified,
    string? profileImage = null,
    DateTime? LockedTill = null);