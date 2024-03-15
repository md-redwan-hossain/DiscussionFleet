using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AnswerCommentConfig : IEntityTypeConfiguration<AnswerComment>
{
    public void Configure(EntityTypeBuilder<AnswerComment> builder)
    {
        builder.ToTable(EntityDbTableNames.AnswerComment);

        builder
            .Property(c => c.Body)
            .HasMaxLength(EntityConstants.CommentBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.AnswerCommentBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    builder.Property(x => x.Body).Metadata.Name,
                    EntityConstants.CommentBodyMinLength
                )
            ));

        builder.HasKey(x => new { x.AnswerId, x.CommenterId });


        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.CommenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}