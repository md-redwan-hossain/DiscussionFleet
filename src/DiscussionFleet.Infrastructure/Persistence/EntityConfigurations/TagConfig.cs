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
        builder.ToTable(DomainEntityDbTableNames.Tag);

        builder
            .Property(c => c.Title)
            .HasMaxLength(DomainEntityConstants.TagTitleMaxLength);

        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.TagTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Tag.Title),
                    DomainEntityConstants.TagTitleMinLength,
                    SqlDataType.VarChar
                )
            ));
    }
}