using DiscussionFleet.Domain.Entities;

namespace DiscussionFleet.Domain.Repositories;

public interface IQuestionRepository : IRepositoryBase<Question, Guid>
{
}