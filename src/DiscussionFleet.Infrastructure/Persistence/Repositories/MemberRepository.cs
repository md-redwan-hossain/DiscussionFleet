using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MemberRepository : Repository<Member, Guid>, IMemberRepository
{
    private readonly IApplicationDbContext _applicationDbContext;

    public MemberRepository(IApplicationDbContext context) : base((DbContext)context)
    {
        _applicationDbContext = context;
    }
}