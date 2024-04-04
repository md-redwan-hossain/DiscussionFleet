using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MemberBadgeConfig : IEntityTypeConfiguration<MemberBadge>
{
    public void Configure(EntityTypeBuilder<MemberBadge> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.MemberBadge);

        builder.HasKey(x => new { x.MemberId, x.BadgeId });

        builder
            .HasOne<Member>()
            .WithMany(x => x.MemberBadges)
            .HasForeignKey(x => x.MemberId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Badge>()
            .WithMany()
            .HasForeignKey(x => x.BadgeId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}