using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class BlogPostRepository : Repository<BlogPost, Guid>, IBlogPostRepository
{
    public BlogPostRepository(ApplicationDbContext context) : base(context)
    {
    }
}