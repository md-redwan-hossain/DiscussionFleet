using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures;
using DiscussionFleet.Domain.Entities;

namespace DiscussionFleet.Web.Models;

public class QuestionSearchViewModel
{
    public byte DataPerPage { get; set; }
    public DataSortOrder SortOrder { get; set; } = new() { Asc = true };
    public QuestionFilterCriteria FilterCriteria { get; set; } = new();
    public QuestionSortCriteria SortCriteria { get; set; } = new() { Newest = true };
    public PaginationViewModel PaginationData { get; set; } = new();
    public HashSet<Guid> SelectedTags { get; set; } = [];
    public IList<QuestionAtSearchResultViewModel> Questions { get; set; } = [];

    public async Task<IList<Tag>> FetchTagsAsync()
    {
        return await Task.FromResult<IList<Tag>>([
            new Tag { Id = Guid.Parse("75576787-f82e-4a7f-97c5-0957bfad83e4"), Title = "CSharp" },
            new Tag { Id = Guid.Parse("4b5f1cee-cbe7-42f9-bdcf-e4182bb53950"), Title = "TypeScript" }
        ]);
    }
}