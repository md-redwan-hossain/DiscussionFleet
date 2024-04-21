using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class TagRepository : Repository<Tag, TagId>, ITagRepository
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }
}