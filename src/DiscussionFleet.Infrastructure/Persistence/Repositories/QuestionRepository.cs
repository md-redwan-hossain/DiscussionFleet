using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class QuestionRepository : Repository<Question, Guid>, IQuestionRepository
{
    public QuestionRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }

    public void Test()
    {
    }
}