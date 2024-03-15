namespace DiscussionFleet.Domain.Entities.Helpers;

public static class EntityConstants
{
    // Question
    public const int QuestionTitleMinLength = 15;
    public const int QuestionTitleMaxLength = 150;
    public const int QuestionBodyMinLength = 220;
    public const int QuestionBodyMaxLength = 30000;
    public const string QuestionBodyMinLengthConstraint = "MinQuestionBodyLength";
    public const string QuestionTitleMinLengthConstraint = "MinQuestionTileLength";

    // Answer
    public const string AnswerBodyMinLengthConstraint = "MinAnswerBodyLength";
    public const int AnswerBodyMinLength = 30;
    public const int AnswerBodyMaxLength = 30000;

    // Comment
    public const string CommentBodyMinLengthConstraint = "MinCommentBodyLength";
    public const int CommentBodyMinLength = 15;
    public const int CommentBodyMaxLength = 600;

    // Tag
    public const string TagTitleMinLengthConstraint = "MinTagTitleLength";
    public const int TagTitleMaxLength = 35;
    public const int TagTitleMinLength = 1;

    // Badge
    public const string BadgeTitleMinLengthConstraint = "MinBadgeTitleLength";
    public const int BadgeTitleMaxLength = 35;
    public const int BadgeTitleMinLength = 1;


    // Blog
    public const string BlogTitleMinLengthConstraint = "MinBlogTitleLength";
    public const string BlogBodyMinLengthConstraint = "MinBlogBodyLength";
    public const int BlogTitleMaxLength = 150;
    public const int BlogTitleMinLength = 1;
    public const int BlogBodyMinLength = 30;
    public const int BlogBodyMaxLength = 50000;
};