using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MultimediaImageRepository : Repository<MultimediaImage, Guid>, IMultimediaImageRepository
{
    public MultimediaImageRepository(IApplicationDbContext context) : base((DbContext)context)
    {
    }
}