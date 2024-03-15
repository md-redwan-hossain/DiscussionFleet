using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class TagRepository : Repository<Tag, Guid>, ITagRepository
{
    public TagRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}