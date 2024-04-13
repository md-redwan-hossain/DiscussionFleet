using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class TagRepository : Repository<Tag, Guid>, ITagRepository
{
    public TagRepository(ApplicationDbContext context) : base(context)
    {
    }
}