using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class BlogPostAuthorConfig : IEntityTypeConfiguration<BlogPostAuthor>
{
    public void Configure(EntityTypeBuilder<BlogPostAuthor> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.BlogPostAuthor);
        
        builder.HasKey(x => new { x.BlogPostId, x.BlogAuthorId });

        builder
            .HasOne<BlogPost>()
            .WithMany(x => x.Authors)
            .HasForeignKey(x => x.BlogPostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.BlogAuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}