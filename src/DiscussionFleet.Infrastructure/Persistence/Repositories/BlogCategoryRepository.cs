using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class BlogCategoryRepository : Repository<BlogCategory, Guid>, IBlogCategoryRepository
{
    public BlogCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}