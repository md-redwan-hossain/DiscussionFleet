using Microsoft.AspNetCore.Identity;

namespace DiscussionFleet.Infrastructure.Identity.Managers;

public class ApplicationUserToken
    : IdentityUserToken<Guid>;