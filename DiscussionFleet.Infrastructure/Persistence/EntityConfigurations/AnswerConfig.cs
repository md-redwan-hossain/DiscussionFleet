using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Generators;
using StringMate.Enums;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AnswerConfig : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.Answer);
        
        builder.Property(e => e.Id)
            .HasConversion(
                convertToProviderExpression: value => value.Data,
                convertFromProviderExpression: value => new AnswerId(value)
            );

        builder
            .Property(c => c.Body)
            .HasMaxLength(DomainEntityConstants.AnswerBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.AnswerBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Answer.Body),
                    DomainEntityConstants.AnswerBodyMinLength,
                    SqlDataType.VarChar
                )
            ));

        builder
            .HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.AnswerGiverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);

    }
}