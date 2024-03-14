using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class BlogPost : Entity<Guid>
{
    public string Title { get; set; }
    public string Summary { get; set; }
    public Guid? CoverImageId { get; set; }
    public string CoverImageFileExtension { get; set; }
    public string Details { get; set; }
    public ICollection<BlogPostCategories> Categories { get; set; }
}