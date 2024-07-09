using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class QuestionVoteConfig : IEntityTypeConfiguration<QuestionVote>
{
    public void Configure(EntityTypeBuilder<QuestionVote> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.QuestionVote);

        builder.HasIndex(x => new { x.VoteGiverId, x.QuestionId }).IsUnique();

        builder
            .HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.VoteGiverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
    }
}