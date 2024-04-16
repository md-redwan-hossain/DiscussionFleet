namespace DiscussionFleet.Application.MembershipFeatures.Enums;

public enum CredentialError : byte
{
    UserNameNotFound = 1,
    PasswordNotMatched,
    ProfileAlreadyConfirmed 
}