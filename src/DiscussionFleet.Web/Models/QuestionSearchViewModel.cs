using DiscussionFleet.Domain.Entities;

namespace DiscussionFleet.Web.Models;

public class QuestionSearchViewModel
{
    public byte DataPerPage { get; set; }
    public string SortOrder { get; set; }
    public string QuestionFilterCriteria { get; set; }
    public string QuestionSortCriteria { get; set; }
    public PaginationViewModel PaginationData { get; set; } = new();
    public IList<Guid> SelectedTags { get; set; } = [];
    public IList<QuestionAtSearchResultViewModel> Questions { get; set; } = [];

    public async Task<IList<Tag>> FetchTagsAsync()
    {
        return await Task.FromResult<IList<Tag>>([new Tag() { Id = Guid.NewGuid(), Title = "CSharp" }]);
    }
}