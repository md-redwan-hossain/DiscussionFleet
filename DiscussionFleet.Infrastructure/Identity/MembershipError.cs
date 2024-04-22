using DiscussionFleet.Application.MembershipFeatures.Interfaces;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity;

public class MembershipError : IMembershipError
{
    public BadOutcomeTag Reason { get; }
    public ICollection<KeyValuePair<string, string>> Errors { get; }
    
    public MembershipError(BadOutcomeTag reason, ICollection<KeyValuePair<string, string>> errors)
    {
        Reason = reason;
        Errors = errors;
    }
}