using DiscussionFleet.Domain.Entities.MemberAggregate;

namespace DiscussionFleet.Domain.Repositories;

public interface IMemberRepository : IRepositoryBase<Member, MemberId>
{
    Task<bool> ReputationUpvoteAsync(MemberId id, int positivePoint);
    Task<bool> ReputationDownVoteAsync(MemberId id, int negativePoint);
}