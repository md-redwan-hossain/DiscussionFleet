using DiscussionFleet.Application.QuestionFeatures.DataTransferObjects;
using DiscussionFleet.Application.TagFeatures.DataTransferObjects;

namespace DiscussionFleet.Web.Models.Question;

public class QuestionAtSearchResultViewModel
{
    public QuestionTitleResponse TitleResponse { get; set; }
    public string Body { get; set; }
    public DateTime LastActivity { get; set; }
    public QuestionAuthorResponse AuthorResponse { get; set; }
    public ICollection<TagWithUrlResponse> Tags { get; set; }
    public QuestionStatsViewModel Stats { get; set; }
}