using System.Security.Claims;
using DiscussionFleet.Application;
using Microsoft.AspNetCore.Authorization;

namespace DiscussionFleet.Infrastructure.Identity;

public class MinimumReputationHandler : AuthorizationHandler<MinimumReputationRequirement>
{
    private readonly IApplicationUnitOfWork _applicationUnitOfWork;

    public MinimumReputationHandler(IApplicationUnitOfWork applicationUnitOfWork)
    {
        _applicationUnitOfWork = applicationUnitOfWork;
    }

    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context,
        MinimumReputationRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId is not null)
        {
            var member = await _applicationUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == Guid.Parse(userId));
            if (member is not null && member.ReputationCount >= requirement.MinimumReputation)
            {
                context.Succeed(requirement);
            }
        }
    }
}