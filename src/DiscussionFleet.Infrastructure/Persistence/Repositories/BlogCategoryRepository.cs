using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class BlogCategoryRepository : Repository<BlogCategory, Guid>, IBlogCategoryRepository
{
    public BlogCategoryRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}