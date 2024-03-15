using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class AnswerRepository : Repository<Answer, Guid>, IAnswerRepository
{
    public AnswerRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}