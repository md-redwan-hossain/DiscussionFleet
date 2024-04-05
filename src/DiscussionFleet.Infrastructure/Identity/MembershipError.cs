using DiscussionFleet.Application.MembershipFeatures;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity;

public class MembershipError : IMembershipError
{
    public BadOutcomeTag Reason { get; }
    public ICollection<KeyValuePair<string, string>> Errors { get; }

#nullable disable
    public MembershipError(BadOutcomeTag reason, ICollection<KeyValuePair<string, string>> errors = null)
    {
        Reason = reason;
        Errors = errors ?? new List<KeyValuePair<string, string>>();
    }
}