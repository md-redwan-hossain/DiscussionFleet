using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class AnswerVoteConfig : IEntityTypeConfiguration<AnswerVote>
{
    public void Configure(EntityTypeBuilder<AnswerVote> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.AnswerVote);
        
        builder.HasIndex(x => new { x.VoteGiverId, x.AnswerId }).IsUnique();
        
        builder
            .HasOne<Answer>()
            .WithMany()
            .HasForeignKey(x => x.AnswerId)
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