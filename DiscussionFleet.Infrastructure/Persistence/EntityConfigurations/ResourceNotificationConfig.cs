using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
using DiscussionFleet.Domain.Entities.ResourceNotificationAggregate;
using DiscussionFleet.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class ResourceNotificationConfig : IEntityTypeConfiguration<ResourceNotification>
{
    public void Configure(EntityTypeBuilder<ResourceNotification> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.ResourceNotification);

        builder.Property(e => e.Id)
            .HasConversion(
                convertToProviderExpression: value => value.Data,
                convertFromProviderExpression: value => new ResourceNotificationId(value)
            );

        builder
            .Property(c => c.Title)
            .HasMaxLength(DomainEntityConstants.QuestionTitleMaxLength);


        builder
            .HasOne<Member>()
            .WithMany()
            .HasForeignKey(x => x.ConsumerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.NoAction);
        
        builder
            .HasOne<Question>()
            .WithMany()
            .HasForeignKey(x => x.QuestionId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}