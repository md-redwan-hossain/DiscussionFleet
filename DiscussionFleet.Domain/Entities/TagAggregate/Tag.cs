using DiscussionFleet.Domain.Common;

namespace DiscussionFleet.Domain.Entities.TagAggregate;

public class Tag : Entity<TagId>
{
    public string Title { get; set; }
}