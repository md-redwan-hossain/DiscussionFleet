using DiscussionFleet.Domain.Entities.UnaryAggregates;

namespace DiscussionFleet.Domain.Repositories;

public interface IAnswerVoteRepository : IRepositoryBase<AnswerVote, Guid>
{
}