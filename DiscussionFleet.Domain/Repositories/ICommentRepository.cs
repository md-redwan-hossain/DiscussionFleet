using DiscussionFleet.Domain.Entities.UnaryAggregates;

namespace DiscussionFleet.Domain.Repositories;

public interface ICommentRepository : IRepositoryBase<Comment, Guid>
{
}