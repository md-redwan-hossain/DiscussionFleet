using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class InternalRuleConfig : IEntityTypeConfiguration<InternalRule>
{
    public void Configure(EntityTypeBuilder<InternalRule> builder)
    {
        builder.ToTable(EntityDbTableNames.InternalRule);
    }
}