using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Membership;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, Guid,
    ApplicationUserClaim, ApplicationUserRole,
    ApplicationUserLogin, ApplicationRoleClaim,
    ApplicationUserToken>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Member> UserProfiles => Set<Member>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}