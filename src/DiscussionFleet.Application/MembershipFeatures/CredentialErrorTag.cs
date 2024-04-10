namespace DiscussionFleet.Application.MembershipFeatures;

public enum CredentialError : byte
{
    UserNameNotFound = 1,
    PasswordNotMatched,
    ProfileAlreadyConfirmed 
}