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
        builder.ToTable(EntityDbTableNames.BlogPost);

        builder
            .Property(x => x.Title)
            .HasMaxLength(EntityConstants.BlogTitleMaxLength);

        builder
            .Property(x => x.Body)
            .HasMaxLength(EntityConstants.BlogBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.BlogTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(BlogPost.Title),
                    EntityConstants.BlogTitleMinLength
                )
            ));


        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.BlogBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(BlogPost.Body),
                    EntityConstants.BlogBodyMinLength
                )
            ));
    }
}