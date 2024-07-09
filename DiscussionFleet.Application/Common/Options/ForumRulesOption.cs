using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record ForumRulesOptions
{
    public const string SectionName = "ForumRulesOptions";
    [Required] public required int MinimumReputationForVote { get; init; }
    [Required] public required int QuestionVotedUp { get; init; }
    [Required] public required int AnswerVotedUp { get; init; }
    [Required] public required int AnswerMarkedAccepted { get; init; }
    [Required] public required int QuestionVotedDown { get; init; }
    [Required] public required int AnswerVotedDown { get; init; }
    [Required] public required int NewQuestion { get; init; }
    [Required] public required int NewAnswer { get; init; }
}