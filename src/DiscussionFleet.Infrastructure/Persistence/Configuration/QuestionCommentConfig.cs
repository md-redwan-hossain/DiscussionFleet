using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class QuestionCommentConfig : IEntityTypeConfiguration<QuestionComment>
{
    public void Configure(EntityTypeBuilder<QuestionComment> builder)
    {
        builder.HasKey(x => new { x.QuestionId, x.CommenterId });

        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.CommenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}