using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class BadgeConfig : IEntityTypeConfiguration<Badge>
{
    public void Configure(EntityTypeBuilder<Badge> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.Badge);

        builder
            .Property(c => c.Title)
            .HasMaxLength(DomainEntityConstants.BadgeTitleMaxLength);

        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.BadgeTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Badge.Title),
                    DomainEntityConstants.BadgeTitleMinLength,
                    SqlDataType.VarChar
                )
            ));
    }
}