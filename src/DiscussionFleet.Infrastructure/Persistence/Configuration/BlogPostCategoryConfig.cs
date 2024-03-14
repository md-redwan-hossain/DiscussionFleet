using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class BlogPostCategoryConfig : IEntityTypeConfiguration<BlogPostCategories>
{
    public void Configure(EntityTypeBuilder<BlogPostCategories> builder)
    {
        builder.HasKey(x => new { x.BlogPostId, x.BlogCategoryId });

        builder
            .HasOne<BlogPost>()
            .WithMany(x => x.Categories)
            .HasForeignKey(x => x.BlogPostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<BlogCategories>()
            .WithMany()
            .HasForeignKey(x => x.BlogCategoryId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}