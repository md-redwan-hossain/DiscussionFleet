using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.QuestionFeatures;

namespace DiscussionFleet.Web.Models;

public class QuestionSearchFilterViewModel
{
    public byte DataPerPage { get; set; }
    public SortOrder SortOrder { get; set; } 
    public QuestionFilterCriteria QuestionFilterCriteria { get; set; } 
    public QuestionSortCriteria QuestionSortCriteria { get; set; } 
    public PaginationViewModel PaginationData { get; set; } 
    public IList<Guid> SelectedTags { get; set; } = [];
}