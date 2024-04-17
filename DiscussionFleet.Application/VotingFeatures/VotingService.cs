namespace DiscussionFleet.Application.VotingFeatures;

public class VotingService : IVotingService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;

    public VotingService(IApplicationUnitOfWork appUnitOfWork)
    {
        _appUnitOfWork = appUnitOfWork;
    }

    public async Task<bool> MemberUpvoteAsync(Guid id, int positivePoint)
    {
        if (positivePoint <= 0) return false;

        var entity = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.Upvote(positivePoint);
        if (!result) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> MemberDownVoteAsync(Guid id, int negativePoint)
    {
        if (negativePoint >= 0) return false;

        var entity = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.DownVote(negativePoint);
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> QuestionUpvoteAsync(Guid id)
    {
        var entity = await _appUnitOfWork.QuestionRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.Upvote();
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> QuestionDownVoteAsync(Guid id)
    {
        var entity = await _appUnitOfWork.QuestionRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.DownVote();
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> AnswerUpvoteAsync(Guid id)
    {
        var entity = await _appUnitOfWork.AnswerRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.Upvote();
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }

    public async Task<bool> AnswerDownVoteAsync(Guid id)
    {
        var entity = await _appUnitOfWork.AnswerRepository.GetOneAsync(x => x.Id == id);
        if (entity is null) return false;

        var result = entity.DownVote();
        if (result is false) return false;

        await _appUnitOfWork.SaveAsync();
        return true;
    }
}