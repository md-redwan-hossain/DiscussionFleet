using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;

namespace DiscussionFleet.Domain.Repositories;

public interface ITagRepository : IRepositoryBase<Tag, TagId>
{
}