using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MemberRepository : Repository<Member, Guid>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }
}