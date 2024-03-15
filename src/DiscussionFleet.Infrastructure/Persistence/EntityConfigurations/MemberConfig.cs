using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Infrastructure.Membership;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MemberConfig : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable(EntityDbTableNames.Member);

        builder.Property(x => x.ReputationCount).HasDefaultValue(1);

        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);
        
        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.MemberMinReputationConstraint,
                cc.GreaterThanOrEqual(
                    builder.Property(x => x.ReputationCount).Metadata.Name, 1
                )
            ));


        builder
            .Property(x => x.FullName)
            .HasMaxLength(EntityConstants.MemberFullNameMaxLength);

        builder
            .Property(x => x.Bio)
            .HasMaxLength(EntityConstants.MemberBioMaxLength);

        builder
            .Property(x => x.Location)
            .HasMaxLength(EntityConstants.MemberLocationMaxLength);

        builder
            .Property(x => x.TwitterHandle)
            .HasMaxLength(EntityConstants.MemberTwitterMaxLength);

        builder
            .Property(x => x.GitHubHandle)
            .HasMaxLength(EntityConstants.MemberGithubMaxLength);

        builder
            .Property(x => x.DisplayName)
            .HasMaxLength(EntityConstants.MemberDisplayNameMaxLength);

        builder
            .Property(x => x.PersonalWebsiteUrl)
            .HasMaxLength(EntityConstants.MemberPersonalWebsiteMaxLength);

        builder
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Member>(x => x.ApplicationUserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<MultimediaImage>()
            .WithOne()
            .HasForeignKey<Member>(x => x.ProfileImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);
    }
}