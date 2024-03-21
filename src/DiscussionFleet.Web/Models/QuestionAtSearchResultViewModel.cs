namespace DiscussionFleet.Web.Models;

public class QuestionAtSearchResultViewModel
{
    public KeyValuePair<string, string> Title { get; set; }
    public string Body { get; set; }
    public DateTime LastActivity { get; set; }
    public KeyValuePair<string, string> Author { get; set; }
    public ICollection<KeyValuePair<string, string>> Tags { get; set; }
    public QuestionStatsViewModel Stats { get; set; }
}