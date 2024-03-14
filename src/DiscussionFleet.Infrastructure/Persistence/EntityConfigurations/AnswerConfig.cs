using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AnswerConfig : IEntityTypeConfiguration<Answer>
{
    public void Configure(EntityTypeBuilder<Answer> builder)
    {
        builder.ToTable(EntityDbTableNames.Answer);

        builder
            .Property(c => c.Body)
            .HasMaxLength(EntityConstants.AnswerBodyMaxLength);

        builder
            .HasOne(x => x.AcceptedAnswer)
            .WithOne()
            .HasForeignKey<AcceptedAnswer>(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}