using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class BlogPostCategories : Timestamp
{
    public Guid BlogPostId { get; set; }
    public Guid BlogCategoryId { get; set; }
}