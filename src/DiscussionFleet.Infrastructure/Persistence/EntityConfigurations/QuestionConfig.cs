using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Enums;
using StringMate.Generators;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class QuestionConfig : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.Question);

        builder
            .Property(c => c.Title)
            .HasMaxLength(DomainEntityConstants.QuestionTitleMaxLength);

        builder
            .Property(c => c.Body)
            .HasMaxLength(DomainEntityConstants.QuestionBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(InfrastructureConstants.DatabaseInUse);

        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.QuestionBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Question.Body),
                    DomainEntityConstants.QuestionBodyMinLength,
                    SqlDataType.VarChar
                )
            ));


        builder.ToTable(b =>
            b.HasCheckConstraint(DomainEntityConstants.QuestionTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Question.Title),
                    DomainEntityConstants.QuestionTitleMinLength,
                    SqlDataType.VarChar
                )
            ));


        builder
            .HasOne(x => x.AcceptedAnswer)
            .WithOne()
            .HasForeignKey<AcceptedAnswer>(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        builder
            .HasMany(x => x.Votes)
            .WithOne()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);


        builder
            .HasOne<Member>()
            .WithOne()
            .HasForeignKey<Question>(x => x.AuthorId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

    }
}