using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Domain;

public interface IUserProfileRepository : IRepositoryBase<Member, Guid>
{
}