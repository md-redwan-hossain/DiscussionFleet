using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record FileBucketOptions
{
    public const string SectionName = "FileBucketOptions";
    [Required] public required string BucketName { get; init; }
}