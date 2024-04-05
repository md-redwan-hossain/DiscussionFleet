using SharpOutcome;

namespace DiscussionFleet.Application.MembershipFeatures;

public interface IMembershipError
{
    BadOutcomeTag Reason { get; }
    ICollection<KeyValuePair<string, string>> Errors { get; }
}