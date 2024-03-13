using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public interface IApplicationDbContext
{
    public DbSet<UserProfile> UserProfiles { get; }
}