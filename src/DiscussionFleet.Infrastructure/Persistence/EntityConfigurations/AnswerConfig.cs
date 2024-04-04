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
        builder.ToTable(DomainEntityDbTableNames.Answer);

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
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}