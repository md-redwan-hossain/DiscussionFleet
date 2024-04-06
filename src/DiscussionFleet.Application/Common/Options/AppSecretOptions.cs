using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record AppSecretOptions
{
    public const string SectionName = "AppSecretOptions";
    [Required] public required string DatabaseUrl { get; init; }
    [Required] public required string? RedisDistributedCacheUrl { get; init; }
    [Required] public required string? RedisStackExchangeUrl { get; init; }
}