using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Infrastructure.Identity.Managers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MemberConfig : IEntityTypeConfiguration<Member>
{
    public void Configure(EntityTypeBuilder<Member> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.Member);

        builder.Property(x => x.ReputationCount).HasDefaultValue(1);

        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.MemberMinReputationConstraint,
                cc.GreaterThanOrEqual(nameof(Member.ReputationCount), 1,
                    SqlDataType.Int)
            ));


        builder
            .Property(x => x.FullName)
            .HasMaxLength(DomainEntityConstants.MemberFullNameMaxLength);

        builder
            .Property(x => x.Bio)
            .HasMaxLength(DomainEntityConstants.MemberBioMaxLength);

        builder
            .Property(x => x.Location)
            .HasMaxLength(DomainEntityConstants.MemberLocationMaxLength);

        builder
            .Property(x => x.TwitterHandle)
            .HasMaxLength(DomainEntityConstants.MemberTwitterMaxLength);

        builder
            .Property(x => x.GithubHandle)
            .HasMaxLength(DomainEntityConstants.MemberGithubMaxLength);
        

        builder
            .Property(x => x.PersonalWebsiteUrl)
            .HasMaxLength(DomainEntityConstants.MemberPersonalWebsiteMaxLength);

        builder
            .HasOne<ApplicationUser>()
            .WithOne()
            .HasForeignKey<Member>(x => x.Id)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<MultimediaImage>()
            .WithOne()
            .HasForeignKey<Member>(x => x.ProfileImageId)
            .IsRequired(false)
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany<ResourceNotification>()
            .WithOne()
            .HasForeignKey(x => x.ConsumerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}