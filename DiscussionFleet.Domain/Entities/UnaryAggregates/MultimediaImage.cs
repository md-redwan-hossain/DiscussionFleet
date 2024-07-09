using DiscussionFleet.Domain.Entities.Abstracts;
using DiscussionFleet.Domain.Entities.Enums;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class MultimediaImage : Entity<Guid>
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