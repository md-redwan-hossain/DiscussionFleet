using DiscussionFleet.Domain.Entities.AnswerAggregate;
using DiscussionFleet.Domain.Repositories;
namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class AnswerRepository : Repository<Answer, Guid>, IAnswerRepository
{
    public AnswerRepository(ApplicationDbContext context) : base(context)
    {
    }
}