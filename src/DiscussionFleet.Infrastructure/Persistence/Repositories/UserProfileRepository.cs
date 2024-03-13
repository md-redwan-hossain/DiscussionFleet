using DiscussionFleet.Domain;
using DiscussionFleet.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class UserProfileRepository : Repository<UserProfile, Guid>, IUserProfileRepository
{
    private readonly IApplicationDbContext _applicationDbContext;

    public UserProfileRepository(IApplicationDbContext context) : base((DbContext)context)
    {
        _applicationDbContext = context;
    }
}