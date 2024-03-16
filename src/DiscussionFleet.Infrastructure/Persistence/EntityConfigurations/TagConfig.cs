using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class TagConfig : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable(EntityDbTableNames.Tag);

        builder
            .Property(c => c.Title)
            .HasMaxLength(EntityConstants.TagTitleMaxLength);

        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.TagTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Tag.Title),
                    EntityConstants.TagTitleMinLength
                )
            ));
    }
}