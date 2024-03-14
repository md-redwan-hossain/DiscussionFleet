using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities;

public class BlogCategory : Entity<Guid>
{
    public string Title { get; set; }
}