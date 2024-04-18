using DiscussionFleet.Domain.Entities.Helpers;
using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Entities.QuestionAggregate;
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