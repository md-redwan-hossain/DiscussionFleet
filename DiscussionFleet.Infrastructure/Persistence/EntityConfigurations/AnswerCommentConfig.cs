using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AnswerCommentConfig : IEntityTypeConfiguration<AnswerComment>
{
    public void Configure(EntityTypeBuilder<AnswerComment> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.AnswerComment);

        builder.HasKey(x => new { x.AnswerId, x.CommentId });

        builder
            .HasOne<Answer>()
            .WithMany()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Comment>()
            .WithMany()
            .HasForeignKey(x => x.CommentId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}