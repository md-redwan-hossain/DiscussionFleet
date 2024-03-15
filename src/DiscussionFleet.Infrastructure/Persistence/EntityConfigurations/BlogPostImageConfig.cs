using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class BlogPostImageConfig : IEntityTypeConfiguration<BlogPostImage>
{
    public void Configure(EntityTypeBuilder<BlogPostImage> builder)
    {
        builder.ToTable(EntityDbTableNames.BlogPostImage);

        builder.HasKey(x => new { x.BlogPostId, x.ImageId });

        builder
            .HasOne<BlogPost>()
            .WithMany(x => x.Images)
            .HasForeignKey(x => x.BlogPostId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<MultimediaImage>()
            .WithMany()
            .HasForeignKey(x => x.ImageId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}