using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class MultimediaImage : Entity<Guid>
{
    public string? Caption { get; set; }
    public string FileExtension { get; set; }
    public string? Location { get; set; }
}