using SharpOutcome;

namespace DiscussionFleet.Application.MembershipFeatures.Interfaces;

public interface IMembershipError
{
    BadOutcomeTag Reason { get; }
    ICollection<KeyValuePair<string, string>> Errors { get; }
}