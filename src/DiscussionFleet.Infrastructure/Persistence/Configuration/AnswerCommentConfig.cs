using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class AnswerCommentConfig : IEntityTypeConfiguration<AnswerComment>
{
    public void Configure(EntityTypeBuilder<AnswerComment> builder)
    {
        builder.HasKey(x => new { x.AnswerId, x.CommenterId });


        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.CommenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}