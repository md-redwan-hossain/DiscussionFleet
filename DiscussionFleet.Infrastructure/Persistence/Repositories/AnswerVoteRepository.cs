using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class AnswerVoteRepository : Repository<AnswerVote, Guid>, IAnswerVoteRepository
{
    public AnswerVoteRepository(ApplicationDbContext context) : base(context)
    {
    }
}