namespace DiscussionFleet.Application.Common.Utils;

public record MemberCachedInformation(string FullName, bool IsVerified, DateTime? LockedTill = null);