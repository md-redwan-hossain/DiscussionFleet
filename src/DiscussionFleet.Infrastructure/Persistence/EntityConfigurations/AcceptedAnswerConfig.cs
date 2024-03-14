using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AcceptedAnswerConfig : IEntityTypeConfiguration<AcceptedAnswer>
{
    public void Configure(EntityTypeBuilder<AcceptedAnswer> builder)
    {
        builder.ToTable(EntityDbTableNames.AcceptedAnswer);

        builder.HasKey(x => new { x.QuestionId, x.AnswerId });


        builder
            .HasOne<Question>()
            .WithOne()
            .HasForeignKey<AcceptedAnswer>(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}