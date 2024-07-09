namespace DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;

public record MemberRegistrationRequest(string FullName, string Email, string Password);