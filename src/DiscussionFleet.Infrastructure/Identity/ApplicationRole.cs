using DiscussionFleet.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace DiscussionFleet.Infrastructure.Identity;

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