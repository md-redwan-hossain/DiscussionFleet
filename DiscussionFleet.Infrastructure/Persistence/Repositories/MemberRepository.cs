using DiscussionFleet.Domain.Entities.MemberAggregate;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MemberRepository : Repository<Member, MemberId>, IMemberRepository
{
    public MemberRepository(ApplicationDbContext context) : base(context)
    {
    }


    public async Task<bool> ReputationUpvoteAsync(MemberId id, int positivePoint)
    {
        if (positivePoint <= 0) return false;

        var entity = await GetOneAsync(
            filter: x => x.Id == id,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.Upvote(positivePoint);
        return result;
    }


    public async Task<bool> ReputationDownVoteAsync(MemberId id, int negativePoint)
    {
        if (negativePoint >= 0) return false;

        var entity = await GetOneAsync(
            filter: x => x.Id == id,
            useSplitQuery: false
        );

        if (entity is null) return false;

        var result = entity.DownVote(negativePoint);
        return result;
    }
}