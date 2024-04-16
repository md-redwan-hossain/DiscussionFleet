using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record CloudQueueOptions
{
    public const string SectionName = "CloudQueueOptions";
    [Required] public required string FifoQueueName { get; init; }
}