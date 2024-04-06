namespace DiscussionFleet.Application.Common.Utils;

public record VerificationEmailHistory(
    DateTime LastTokenSentAtUtc,
    uint TotalTokenIssued,
    uint ConsecutiveFailedRequest,
    DateTime? EnableRetryAtUtc
);