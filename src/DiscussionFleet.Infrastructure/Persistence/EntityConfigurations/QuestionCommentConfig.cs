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
        builder.ToTable(DomainEntityDbTableNames.QuestionComment);

        builder
            .Property(c => c.Body)
            .HasMaxLength(DomainEntityConstants.CommentBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.QuestionCommentBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(QuestionComment.Body),
                    DomainEntityConstants.CommentBodyMinLength,
                    SqlDataType.VarChar
                )
            ));

        builder.HasKey(x => new { x.QuestionId, x.CommenterId });

    }
}