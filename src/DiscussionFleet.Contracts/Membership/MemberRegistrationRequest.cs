namespace DiscussionFleet.Contracts.Membership;

public record MemberRegistrationRequest(string FullName, string Email, string Password);