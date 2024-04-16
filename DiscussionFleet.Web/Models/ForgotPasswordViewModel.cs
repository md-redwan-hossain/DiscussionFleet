using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Web.Models;

public class ForgotPasswordViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }
}