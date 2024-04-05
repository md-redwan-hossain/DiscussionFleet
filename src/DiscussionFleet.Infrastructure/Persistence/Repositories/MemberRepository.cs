using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MemberRepository : Repository<Member, Guid>, IMemberRepository
{
    

    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }
}