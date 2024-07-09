using DiscussionFleet.Domain.Entities.UnaryAggregates;

namespace DiscussionFleet.Domain.Repositories;

public interface IQuestionVoteRepository : IRepositoryBase<QuestionVote, Guid>
{
}