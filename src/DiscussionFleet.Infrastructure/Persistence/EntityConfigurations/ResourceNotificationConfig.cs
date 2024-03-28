using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class ResourceNotificationConfig : IEntityTypeConfiguration<ResourceNotification>
{
    public void Configure(EntityTypeBuilder<ResourceNotification> builder)
    {
        builder.ToTable(EntityDbTableNames.ResourceNotification);

        builder
            .Property(c => c.SourceTitle)
            .HasMaxLength(EntityConstants.QuestionTitleMaxLength);
    }
}