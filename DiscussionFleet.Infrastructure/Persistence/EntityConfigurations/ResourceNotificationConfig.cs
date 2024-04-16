using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class ResourceNotificationConfig : IEntityTypeConfiguration<ResourceNotification>
{
    public void Configure(EntityTypeBuilder<ResourceNotification> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.ResourceNotification);

        builder
            .Property(c => c.SourceTitle)
            .HasMaxLength(DomainEntityConstants.QuestionTitleMaxLength);
    }
}