namespace DiscussionFleet.Web.Models.QuestionWithRelated;

public class ReadCommentViewModel
{
    public string AuthorName { get; set; }
    public string Body { get; set; }
    public DateTime LastActivityUtc { get; set; }
}