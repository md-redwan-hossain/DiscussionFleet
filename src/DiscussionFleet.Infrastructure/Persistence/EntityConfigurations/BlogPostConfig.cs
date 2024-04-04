using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class BlogPostConfig : IEntityTypeConfiguration<BlogPost>
{
    public void Configure(EntityTypeBuilder<BlogPost> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.BlogPost);

        builder
            .Property(x => x.Title)
            .HasMaxLength(DomainEntityConstants.BlogTitleMaxLength);

        builder
            .Property(x => x.Body)
            .HasMaxLength(DomainEntityConstants.BlogBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.BlogTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(BlogPost.Title),
                    DomainEntityConstants.BlogTitleMinLength,
                    SqlDataType.VarChar
                )
            ));


        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.BlogBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(BlogPost.Body),
                    DomainEntityConstants.BlogBodyMinLength,
                    SqlDataType.VarChar
                )
            ));
    }
}