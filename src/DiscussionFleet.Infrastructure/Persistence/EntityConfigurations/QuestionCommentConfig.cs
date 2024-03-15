using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;


namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class QuestionCommentConfig : IEntityTypeConfiguration<QuestionComment>
{
    public void Configure(EntityTypeBuilder<QuestionComment> builder)
    {
        builder.ToTable(EntityDbTableNames.QuestionComment);

        builder
            .Property(c => c.Body)
            .HasMaxLength(EntityConstants.CommentBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.QuestionCommentBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    builder.Property(x => x.Body).Metadata.Name,
                    EntityConstants.CommentBodyMinLength
                )
            ));

        builder.HasKey(x => new { x.QuestionId, x.CommenterId });

        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.CommenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}