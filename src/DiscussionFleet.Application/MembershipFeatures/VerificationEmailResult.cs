namespace DiscussionFleet.Application.MembershipFeatures;

public struct VerificationEmailResult<T>
{
    public bool IsSuccess { get; private set; }
    public T? Reason { get; init; }
}