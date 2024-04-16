using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class SavedQuestionConfig : IEntityTypeConfiguration<SavedQuestion>
{
    public void Configure(EntityTypeBuilder<SavedQuestion> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.SavedQuestion);

        builder.HasKey(x => new { x.QuestionId, x.MemberId });

        builder
            .HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Member>()
            .WithMany(x => x.SavedQuestions)
            .HasForeignKey(x => x.MemberId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}