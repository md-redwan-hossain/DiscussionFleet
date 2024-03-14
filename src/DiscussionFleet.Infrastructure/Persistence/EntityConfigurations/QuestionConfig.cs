using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

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

        builder
            .HasMany(x => x.Comments)
            .WithOne()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}