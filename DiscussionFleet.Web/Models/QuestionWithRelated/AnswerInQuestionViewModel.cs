namespace DiscussionFleet.Web.Models.QuestionWithRelated;

public class AnswerInQuestionViewModel
{
    public int VoteCount { get; set; }
    public int CommentCount { get; set; }
    public string AuthorName { get; set; }
    public string? ProfilePicUrl { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public int AuthorReputation { get; set; }
    public bool IsAccepted { get; set; }

    public ICollection<CommentViewModel> Comments { get; set; } = [];
}