namespace DiscussionFleet.Application.MembershipFeatures;

public enum ResendEmailErrorReason : byte
{
    EntityNotFound,
    Unknown,
    TooEarly
}