using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record GoogleRecaptchaOptions
{
    public const string SectionName = "GoogleRecaptchaOptions";
    [Required] public required string SiteKey { get; init; }
    [Required] public required string SecretKey { get; init; }
    [Required] public required string Version { get; init; }
}