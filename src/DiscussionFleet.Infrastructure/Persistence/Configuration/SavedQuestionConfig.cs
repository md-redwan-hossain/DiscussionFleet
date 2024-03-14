using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class SavedQuestionConfig : IEntityTypeConfiguration<SavedQuestion>
{
    public void Configure(EntityTypeBuilder<SavedQuestion> builder)
    {
        builder.HasKey(x => new { x.QuestionId, x.UserId });

        builder
            .HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<UserProfile>()
            .WithMany(x => x.SavedQuestions)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}