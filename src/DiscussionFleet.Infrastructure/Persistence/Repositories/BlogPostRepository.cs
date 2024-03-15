using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class BlogPostRepository : Repository<BlogPost, Guid>, IBlogPostRepository
{
    public BlogPostRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}