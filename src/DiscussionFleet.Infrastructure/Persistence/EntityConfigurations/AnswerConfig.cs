using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StringMate.Generators;
using StringMate.Enums;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AnswerConfig : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable(EntityDbTableNames.Answer);

        builder
            .Property(c => c.Body)
            .HasMaxLength(EntityConstants.AnswerBodyMaxLength);


        var cc = new SqlCheckConstrainGenerator(RDBMS.SqlServer);

        builder.ToTable(b =>
            b.HasCheckConstraint(EntityConstants.AnswerBodyMinLengthConstraint,
                cc.GreaterThanOrEqual(
                    nameof(Answer.Body),
                    EntityConstants.AnswerBodyMinLength
                )
            ));


        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}