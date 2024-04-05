namespace DiscussionFleet.Domain.Entities.Helpers;

public static class DomainEntityConstants
{
    // Member
    public const string MemberMinReputationConstraint = "MinMemberReputation";
    public const int MemberLocationMaxLength = 100;
    public const int MemberDisplayNameMaxLength = 100;
    public const int MemberFullNameMaxLength = 100;
    public const int MemberBioMaxLength = 1000;
    public const int MemberPersonalWebsiteMaxLength = 200;
    public const int MemberTwitterMaxLength = 200;
    public const int MemberGithubMaxLength = 200;


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
    public const string AnswerCommentBodyMinLengthConstraint = "MinAnswerCommentBodyLength";
    public const string QuestionCommentBodyMinLengthConstraint = "MinQuestionCommentBodyLength";
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

    // MultimediaImage
    public const int MultimediaImageCaptionMaxLength = 200;
    public const int MultimediaImageFileExtensionMaxLength = 10;
}