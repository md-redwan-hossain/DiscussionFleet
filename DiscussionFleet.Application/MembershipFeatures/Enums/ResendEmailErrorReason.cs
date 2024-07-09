namespace DiscussionFleet.Application.MembershipFeatures.Enums;

public enum ResendEmailErrorReason : byte
{
    EntityNotFound,
    Unknown,
    TooEarly
}