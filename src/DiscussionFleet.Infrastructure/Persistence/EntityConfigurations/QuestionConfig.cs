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
        builder.ToTable(EntityDbTableNames.Question);
        
        builder
            .Property(c => c.Title)
            .HasMaxLength(EntityConstants.QuestionTitleMaxLength);

        builder
            .Property(c => c.Body)
            .HasMaxLength(EntityConstants.QuestionBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.QuestionBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    builder.Property(x => x.Body).Metadata.Name,
                    EntityConstants.QuestionBodyMinLength
                )
            ));


        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.QuestionTitleMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    builder.Property(x => x.Title).Metadata.Name,
                    EntityConstants.QuestionTitleMinLength
                )
            ));

        builder
            .HasOne(x => x.AcceptedAnswer)
            .WithOne()
            .HasForeignKey<AcceptedAnswer>(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}