using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class ForumRuleConfig : IEntityTypeConfiguration<ForumRule>
{
    public void Configure(EntityTypeBuilder<ForumRule> builder)
    {
        builder.ToTable(EntityDbTableNames.ForumRule);
    }
}