using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class QuestionVoteConfig : IEntityTypeConfiguration<QuestionVote>
{
    public void Configure(EntityTypeBuilder<QuestionVote> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.QuestionVote);

        builder.HasKey(x => new { x.QuestionId, x.VoteGiverId });

        builder
            .HasOne<Question>()
            .WithMany(x => x.Votes)
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.VoteGiverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}