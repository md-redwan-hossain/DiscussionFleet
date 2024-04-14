using DiscussionFleet.Domain.Entities.MemberAggregate;

namespace DiscussionFleet.Domain.Repositories;

public interface IMemberRepository : IRepositoryBase<Member, Guid>
{
}