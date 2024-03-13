using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class UserProfileConfig : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<UserProfile>(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}