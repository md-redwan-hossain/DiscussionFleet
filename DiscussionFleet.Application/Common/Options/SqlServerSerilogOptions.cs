using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Logging;

namespace DiscussionFleet.Application.Common.Options;

public record SqlServerSerilogOptions
{
    public const string SectionName = "SqlServerSerilogOptions";
    [Required] public required string TableName { get; init; }
    [Required] public required string SchemaName { get; init; }
    [Required] public required bool AutoCreateSqlTable { get; init; }
    [Required] public required string MinimumLogLevel { get; init; }
}