using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Entities.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DiscussionFleet.Infrastructure.Persistence.EntityConfigurations;

public class MultimediaImageConfig : IEntityTypeConfiguration<MultimediaImage>
{
    public void Configure(EntityTypeBuilder<MultimediaImage> builder)
    {
        builder.ToTable(EntityDbTableNames.MultimediaImage);

        builder
            .Property(x => x.Caption)
            .HasMaxLength(EntityConstants.MultimediaImageCaptionMaxLength);

        builder
            .Property(x => x.FileExtension)
            .HasMaxLength(EntityConstants.MultimediaImageFileExtensionMaxLength);
    }
}