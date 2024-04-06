using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Application.Common.Options;

public record SmtpOptions
{
    public const string SectionName = "SmtpOptions";
    [Required] public string SenderName { get; set; }
    [Required] public string SenderEmail { get; set; }
    [Required] public string Host { get; set; }
    [Required] public string Username { get; set; }
    [Required] public string Password { get; set; }
    [Required] public int Port { get; set; }
    [Required] public bool UseSSL { get; set; }
}