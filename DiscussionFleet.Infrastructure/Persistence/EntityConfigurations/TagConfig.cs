using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Helpers;
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


        builder.Property(e => e.Id)
            .HasConversion(
                convertToProviderExpression: value => value.Data,
                convertFromProviderExpression: value => new TagId(value)
            );

        builder.HasIndex(x => x.Title).IsUnique();

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