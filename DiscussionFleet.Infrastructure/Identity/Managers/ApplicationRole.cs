using Microsoft.AspNetCore.Identity;

namespace DiscussionFleet.Infrastructure.Identity.Managers;

public class ApplicationRole : IdentityRole<Guid>
{
    public ApplicationRole()
    {
    }

    public ApplicationRole(string roleName)
        : base(roleName)
    {
    }
}