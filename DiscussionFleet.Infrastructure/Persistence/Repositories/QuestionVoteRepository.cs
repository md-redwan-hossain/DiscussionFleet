using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class QuestionVoteRepository : Repository<QuestionVote, Guid>, IQuestionVoteRepository
{
    public QuestionVoteRepository(ApplicationDbContext context) : base(context)
    {
    }
}