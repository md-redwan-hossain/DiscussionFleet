using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.Configuration;

public class AcceptedAnswerConfig : IEntityTypeConfiguration<AcceptedAnswer>
{
    public void Configure(EntityTypeBuilder<AcceptedAnswer> builder)
    {
        builder.HasKey(x => new { x.QuestionId, x.AnswerId });
    }
}