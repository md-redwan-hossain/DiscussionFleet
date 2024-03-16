using Microsoft.AspNetCore.Identity;

namespace DiscussionFleet.Infrastructure.Identity;

public class ApplicationUserToken
    : IdentityUserToken<Guid>;