using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.MultimediaImageAggregate;

public class MultimediaImage : Entity<MultimediaImageId>
{
    public string? Caption { get; set; }
    public ImagePurpose Purpose { get; set; }
    public string FileExtension { get; set; }
    public string? Location { get; set; }

    public string ImageNameResolver(bool includeDot = false)
    {
        return includeDot
            ? $"{Purpose.ToString()}_{Id}.{FileExtension}"
            : $"{Purpose.ToString()}_{Id}{FileExtension}";
    }
}