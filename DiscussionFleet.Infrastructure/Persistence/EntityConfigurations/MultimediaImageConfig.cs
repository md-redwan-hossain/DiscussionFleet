using DiscussionFleet.Domain.Entities.MultimediaImageAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MultimediaImageConfig : IEntityTypeConfiguration<MultimediaImage>
{
    public void Configure(EntityTypeBuilder<MultimediaImage> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.MultimediaImage);
        
        builder.Property(e => e.Id)
            .HasConversion(
                convertToProviderExpression: value => value.Data,
                convertFromProviderExpression: value => new MultimediaImageId(value)
            );

        builder
            .Property(x => x.Caption)
            .HasMaxLength(DomainEntityConstants.MultimediaImageCaptionMaxLength);

        builder
            .Property(x => x.FileExtension)
            .HasMaxLength(DomainEntityConstants.MultimediaImageFileExtensionMaxLength);
    }
}