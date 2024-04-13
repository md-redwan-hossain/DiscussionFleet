using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MultimediaImageRepository : Repository<MultimediaImage, Guid>, IMultimediaImageRepository
{
    public MultimediaImageRepository(ApplicationDbContext context) : base(context)
    {
    }
}