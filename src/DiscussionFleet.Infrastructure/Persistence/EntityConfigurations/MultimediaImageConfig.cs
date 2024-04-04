using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MultimediaImageConfig : IEntityTypeConfiguration<MultimediaImage>
{
    public void Configure(EntityTypeBuilder<MultimediaImage> builder)
    {
        builder.ToTable(DomainEntityDbTableNames.MultimediaImage);

        builder
            .Property(x => x.Caption)
            .HasMaxLength(DomainEntityConstants.MultimediaImageCaptionMaxLength);

        builder
            .Property(x => x.FileExtension)
            .HasMaxLength(DomainEntityConstants.MultimediaImageFileExtensionMaxLength);
    }
}