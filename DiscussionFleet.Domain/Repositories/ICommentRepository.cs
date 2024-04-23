using DiscussionFleet.Domain.Entities.CommentAggregate;

namespace DiscussionFleet.Domain.Repositories;

public interface ICommentRepository : IRepositoryBase<Comment, CommentId>
{
}