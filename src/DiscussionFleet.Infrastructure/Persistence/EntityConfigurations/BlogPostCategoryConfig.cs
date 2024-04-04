using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class BlogPostCategoryConfig : IEntityTypeConfiguration<BlogPostCategory>
{
    public void Configure(EntityTypeBuilder<BlogPostCategory> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.BlogPostCategory);
        
        builder.HasKey(x => new { x.BlogPostId, x.BlogCategoryId });

        builder
            .HasOne<BlogPost>()
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.BlogPostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<BlogCategory>()
            .WithMany()
            .HasForeignKey(x => x.BlogCategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}