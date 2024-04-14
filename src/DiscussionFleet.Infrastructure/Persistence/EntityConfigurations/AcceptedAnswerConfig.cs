using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AcceptedAnswerConfig : IEntityTypeConfiguration<AcceptedAnswer>
{
    public void Configure(EntityTypeBuilder<AcceptedAnswer> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.AcceptedAnswer);

        builder.HasKey(x => new { x.QuestionId, x.AnswerId });

        builder
            .HasOne<Answer>()
            .WithOne()
            .HasForeignKey<AcceptedAnswer>(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}