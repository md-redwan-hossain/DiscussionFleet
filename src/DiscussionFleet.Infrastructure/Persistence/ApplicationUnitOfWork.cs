using DiscussionFleet.Application;
using DiscussionFleet.Domain;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence;

public class ApplicationUnitOfWork : UnitOfWork, IApplicationUnitOfWork
{
    public ApplicationUnitOfWork(IApplicationDbContext dbContext,
        IUserProfileRepository userProfileRepository)
        : base((DbContext)dbContext)
    {
        UserProfileRepository = userProfileRepository;
    }

    public IUserProfileRepository UserProfileRepository { get; }
}