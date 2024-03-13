using Microsoft.AspNetCore.Identity;

namespace DiscussionFleet.Infrastructure.Membership;

public class ApplicationUserToken
    : IdentityUserToken<Guid>;