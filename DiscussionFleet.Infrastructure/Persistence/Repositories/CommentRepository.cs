using DiscussionFleet.Domain.Entities.CommentAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class CommentRepository : Repository<Comment, CommentId>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context)
    {
    }
}