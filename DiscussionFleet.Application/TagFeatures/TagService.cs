using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.TagFeatures.DataTransferObjects;
using DiscussionFleet.Domain.Entities.TagAggregate;
using DiscussionFleet.Domain.Entities.UnaryAggregates;
using SharpOutcome;

namespace DiscussionFleet.Application.TagFeatures;

public class TagService : ITagService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly IGuidProvider _guidProvider;
    private readonly IDateTimeProvider _dateTimeProvider;

    public TagService(IApplicationUnitOfWork appUnitOfWork, IGuidProvider guidProvider,
        IDateTimeProvider dateTimeProvider)
    {
        _appUnitOfWork = appUnitOfWork;
        _guidProvider = guidProvider;
        _dateTimeProvider = dateTimeProvider;
    }


    public async Task<Outcome<ICollection<Tag>, DuplicateTagError>> CreateMany(TagCreateRequest dto)
    {
        var duplicateData = await _appUnitOfWork.TagRepository.GetAllAsync(
            filter: x => dto.TagTitles.Contains(x.Title),
            subsetSelector: x => x.Title,
            orderBy: x => x.Id,
            useSplitQuery: false
        );

        if (duplicateData.Any())
        {
            return new DuplicateTagError(duplicateData);
        }

        List<Tag> tags = [];

        foreach (var dtoTagTitle in dto.TagTitles)
        {
            var tag = new Tag { Id = new TagId(_guidProvider.SortableGuid()), Title = dtoTagTitle };
            tag.SetCreatedAtUtc(_dateTimeProvider.CurrentUtcTime);
            tags.Add(tag);
        }

        await _appUnitOfWork.TagRepository.CreateManyAsync(tags);
        await _appUnitOfWork.SaveAsync();

        return tags;
    }
}