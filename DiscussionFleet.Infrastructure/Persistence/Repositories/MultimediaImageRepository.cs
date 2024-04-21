using DiscussionFleet.Domain.Entities.MultimediaImageAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using DiscussionFleet.Domain.Repositories;

namespace DiscussionFleet.Infrastructure.Persistence.Repositories;

public class MultimediaImageRepository : Repository<MultimediaImage, MultimediaImageId>, IMultimediaImageRepository
{
    public MultimediaImageRepository(ApplicationDbContext context) : base(context)
    {
    }
}