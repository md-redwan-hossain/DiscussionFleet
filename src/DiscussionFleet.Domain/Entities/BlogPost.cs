using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class BlogPost : Entity<Guid>
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public Guid? CoverImageId { get; set; }
    public string CoverImageFileExtension { get; set; }
    public string Body { get; set; }
    public ICollection<BlogPostCategory> Categories { get; set; }
    public ICollection<BlogPostImage> Images { get; set; }
    public ICollection<BlogPostAuthor> Authors { get; set; }
}