using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Infrastructure.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MemberConfig : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable(EntityDbTableNames.Member);

        builder.Property(x => x.ReputationCount).HasDefaultValue(1);

        builder
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Member>(x => x.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}