using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class SavedAnswerConfig : IEntityTypeConfiguration<SavedAnswer>
{
    public void Configure(EntityTypeBuilder<SavedAnswer> builder)
    {
        builder.HasKey(x => new { x.AnswerId, x.UserId });

        builder
            .HasOne<Answer>()
            .WithMany()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<UserProfile>()
            .WithMany(x => x.SavedAnswers)
            .HasForeignKey(x => x.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}