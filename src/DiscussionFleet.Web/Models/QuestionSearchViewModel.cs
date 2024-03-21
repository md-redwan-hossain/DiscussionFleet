using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures;
using DiscussionFleet.Domain.Entities;

namespace DiscussionFleet.Web.Models;

public class QuestionSearchViewModel
{
    public QuestionSearchFilterViewModel FilterData { get; set; }
    public IList<QuestionAtSearchResultViewModel> Questions { get; set; } = [];

    public async Task<IList<Tag>> FetchTagsAsync()
    {
        return await Task.FromResult<IList<Tag>>([new Tag() { Id = Guid.NewGuid(), Title = "CSharp" }]);
    }
}