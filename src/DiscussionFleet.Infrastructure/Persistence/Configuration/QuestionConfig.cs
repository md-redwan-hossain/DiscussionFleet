using DiscussionFleet.Domain;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Abstracts;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class QuestionConfig : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder
            .Property(c => c.Title)
            .HasMaxLength(EntityConstants.QuestionTitleMaxLength);

        builder
            .Property(c => c.Body)
            .HasMaxLength(EntityConstants.QuestionBodyMaxLength);

        builder
            .HasMany(x => x.QuestionComments)
            .WithOne()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}