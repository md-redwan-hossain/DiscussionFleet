using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class ForumRuleConfig : IEntityTypeConfiguration<ForumRule>
{
    public void Configure(EntityTypeBuilder<ForumRule> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.ForumRule);
    }
}