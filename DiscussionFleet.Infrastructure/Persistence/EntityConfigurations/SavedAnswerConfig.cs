using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class SavedAnswerConfig : IEntityTypeConfiguration<SavedAnswer>
{
    public void Configure(EntityTypeBuilder<SavedAnswer> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.SavedAnswer);

        builder.HasKey(x => new { x.AnswerId, x.MemberId });

        builder
            .HasOne<Answer>()
            .WithMany()
            .HasForeignKey(x => x.AnswerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Member>()
            .WithMany(x => x.SavedAnswers)
            .HasForeignKey(x => x.MemberId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}