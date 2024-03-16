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
        builder.ToTable(EntityDbTableNames.Badge);

        builder
            .Property(c => c.Title)
            .HasMaxLength(EntityConstants.BadgeTitleMaxLength);

        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.BadgeTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Badge.Title),
                    EntityConstants.BadgeTitleMinLength
                )
            ));
    }
}