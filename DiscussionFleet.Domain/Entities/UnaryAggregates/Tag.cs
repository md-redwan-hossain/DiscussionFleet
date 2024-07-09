using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class Tag : Entity<Guid>
{
    public string Title { get; set; }
}