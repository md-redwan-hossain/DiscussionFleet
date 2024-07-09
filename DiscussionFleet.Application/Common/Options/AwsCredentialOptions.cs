﻿using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record AwsCredentialOptions
{
    public const string SectionName = "AwsCredentialOptions";
    [Required] public required string AccessKey { get; init; }
    [Required] public required string SecretKey { get; init; }
}