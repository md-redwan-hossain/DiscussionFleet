using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.MultimediaImageAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Infrastructure.Identity.Managers;
using EntityFramework.Exceptions.SqlServer;
using Microsoft.AspNetCore.DataProtection.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public class ApplicationDbContext : IdentityDbContext<
    ApplicationUser, ApplicationRole, Guid,
    ApplicationUserClaim, ApplicationUserRole,
    ApplicationUserLogin, ApplicationRoleClaim,
    ApplicationUserToken>, IDataProtectionKeyContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<DataProtectionKey> DataProtectionKeys => Set<DataProtectionKey>();
    public DbSet<Member> Members => Set<Member>();
    public DbSet<Answer> Answers => Set<Answer>();
    public DbSet<Question> Questions => Set<Question>();
    public DbSet<MultimediaImage> MultimediaImages => Set<MultimediaImage>();
    public DbSet<Tag> Tags => Set<Tag>();
    public DbSet<AnswerVote> AnswerVotes => Set<AnswerVote>();
    public DbSet<QuestionVote> QuestionVotes => Set<QuestionVote>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseExceptionProcessor();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}