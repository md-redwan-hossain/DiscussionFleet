namespace DiscussionFleet.Application.Common.Utils;

public record EmailHistory(string Token, DateTime SavedAtUtc, DateTime LastTokenSentAtUtc);