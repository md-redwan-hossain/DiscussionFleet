namespace DiscussionFleet.Application.MemberReputationFeatures;

public class MemberReputationService : IMemberReputationService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;

    public MemberReputationService(IApplicationUnitOfWork appUnitOfWork)
    {
        _appUnitOfWork = appUnitOfWork;
    }

    public async Task<bool> UpvoteAsync(Guid id, int positivePoint)
    {
        if (positivePoint <= 0) return false;

        var entity = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.Upvote(positivePoint);
        if (!result) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> DownVoteAsync(Guid id, int negativePoint)
    {
        if (negativePoint >= 0) return false;

        var entity = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.DownVote(negativePoint);
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }
}