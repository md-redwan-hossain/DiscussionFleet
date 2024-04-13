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
        builder.ToTable(DomainEntityDbTableNames.AnswerComment);

        builder
            .Property(c => c.Body)
            .HasMaxLength(DomainEntityConstants.CommentBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.AnswerCommentBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(AnswerComment.Body),
                    DomainEntityConstants.CommentBodyMinLength,
                    SqlDataType.VarChar
                )
            ));

        builder.HasKey(x => new { x.AnswerId, x.CommenterId });


        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.CommenterId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}