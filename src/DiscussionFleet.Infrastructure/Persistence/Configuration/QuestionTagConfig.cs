using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class QuestionTagConfig : IEntityTypeConfiguration<QuestionTag>
{
    public void Configure(EntityTypeBuilder<QuestionTag> builder)
    {
        builder.HasKey(x => new { x.QuestionId, x.TagId });

        builder
            .HasOne<Question>()
            .WithMany(x => x.Tags)
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Tag>()
            .WithMany()
            .HasForeignKey(x => x.TagId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}