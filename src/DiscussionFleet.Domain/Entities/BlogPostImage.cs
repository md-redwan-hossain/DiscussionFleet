using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class BlogPostImage : Timestamp
{
    public Guid BlogPostId { get; set; }
    public Guid ImageId { get; set; }
}