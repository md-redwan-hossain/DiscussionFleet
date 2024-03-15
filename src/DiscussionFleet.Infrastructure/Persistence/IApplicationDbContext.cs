using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public interface IApplicationDbContext
{
    public DbSet<Member> Members { get; }
    public DbSet<Answer> Answers { get; }
    public DbSet<Question> Questions { get; }
    public DbSet<BlogPost> BlogPosts { get; }
    public DbSet<Tag> Tags { get; }
    public DbSet<BlogCategory> BlogCategories { get; }
}