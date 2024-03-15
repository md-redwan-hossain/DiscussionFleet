using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class BlogPostAuthor : Timestamp
{
    public Guid BlogPostId { get; set; }
    public Guid BlogAuthorId { get; set; }
}