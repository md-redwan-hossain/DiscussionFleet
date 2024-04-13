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
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.VoteGiverId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}