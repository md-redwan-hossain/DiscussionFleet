using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public interface IApplicationDbContext
{
    public DbSet<Member> UserProfiles { get; }
}