using DiscussionFleet.Domain.Entities.Abstracts;

namespace DiscussionFleet.Domain.Entities.UnaryAggregates;

public class Badge : Entity<Guid>
{
    public string Title { get; set; }
    public int GivenCount { get; set; }
}