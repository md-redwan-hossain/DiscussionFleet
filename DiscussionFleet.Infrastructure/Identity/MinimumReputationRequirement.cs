using Microsoft.AspNetCore.Authorization;

namespace DiscussionFleet.Infrastructure.Identity;

public class MinimumReputationRequirement : IAuthorizationRequirement
{
    public int MinimumReputation { get; }

    public MinimumReputationRequirement(int minimumReputation)
    {
        MinimumReputation = minimumReputation;
    }
}