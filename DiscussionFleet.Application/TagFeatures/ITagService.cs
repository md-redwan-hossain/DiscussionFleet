using DiscussionFleet.Application.TagFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.TagAggregate;
using SharpOutcome;

namespace DiscussionFleet.Application.TagFeatures;

public interface ITagService
{
        Task<Outcome<ICollection<Tag>, DuplicateTagError>> CreateMany(TagCreateRequest dto);
}