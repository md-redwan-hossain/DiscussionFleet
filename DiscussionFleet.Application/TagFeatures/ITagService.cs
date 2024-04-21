using DiscussionFleet.Application.TagFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using SharpOutcome;

namespace DiscussionFleet.Application.TagFeatures;

public interface ITagService
{
        Task<Outcome<ICollection<Tag>, DuplicateTagError>> CreateMany(TagCreateRequest dto);
}