namespace DiscussionFleet.Contracts.Membership;

public record MemberUpdateRequest(
    string FullName,
    string? Location,
    string? Bio,
    string? PersonalWebsiteUrl,
    string? TwitterHandle,
    string? GitHubHandle
);