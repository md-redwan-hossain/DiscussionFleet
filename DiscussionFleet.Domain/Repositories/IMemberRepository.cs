using DiscussionFleet.Domain.Entities.MemberAggregate;

namespace DiscussionFleet.Domain.Repositories;

public interface IMemberRepository : IRepositoryBase<Member, Guid>
{
    Task<bool> ReputationUpvoteAsync(Guid id, int positivePoint);
    Task<bool> ReputationDownVoteAsync(Guid id, int negativePoint);
}