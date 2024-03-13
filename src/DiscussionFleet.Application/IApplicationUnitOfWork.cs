using DiscussionFleet.Domain;

namespace DiscussionFleet.Application;

public interface IApplicationUnitOfWork
{
    public IUserProfileRepository UserProfileRepository { get; }
}