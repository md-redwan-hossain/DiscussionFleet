using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Web.Models;

public class ConfirmAccountViewModel
{
    [Required] public string Code { get; set; }
    public uint TryAgainAfterSeconds { get; set; } = 5;
}