using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record AppOptions
{
    public const string SectionName = "AppOptions";
    [Required] public required string DatabaseUrl { get; init; }
    [Required] public required string? RedisCacheUrl { get; init; }
}