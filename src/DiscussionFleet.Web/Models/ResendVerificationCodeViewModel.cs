using System.ComponentModel.DataAnnotations;

namespace DiscussionFleet.Web.Models;

public class ResendVerificationCodeViewModel
{
    [Required]
    [EmailAddress]
    [Display(Name = "Email")]
    public string Email { get; set; }


    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    public bool HasError { get; set; }
    
}