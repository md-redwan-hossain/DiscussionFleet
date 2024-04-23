using DiscussionFleet.Domain.Entities.CommentAggregate;
using DiscussionFleet.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class CommentConfig : IEntityTypeConfiguration<Comment>
{
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.Comment);


        builder
            .Property(c => c.Body)
            .HasMaxLength(DomainEntityConstants.CommentBodyMaxLength);

        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.QuestionCommentBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Comment.Body),
                    DomainEntityConstants.CommentBodyMinLength,
                    SqlDataType.VarChar
                )
            ));
    }
}