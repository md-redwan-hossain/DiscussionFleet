using DiscussionFleet.Domain.Entities.CommentAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class QuestionCommentConfig : IEntityTypeConfiguration<QuestionComment>
{
    public void Configure(EntityTypeBuilder<QuestionComment> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.QuestionComment);

        builder.HasKey(x => new { x.QuestionId, x.CommentId });

        builder
            .HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
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