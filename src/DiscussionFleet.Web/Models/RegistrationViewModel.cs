using System.ComponentModel.DataAnnotations;
using Autofac;

namespace DiscussionFleet.Web.Models;

public class RegistrationViewModel
{
    private ILifetimeScope _scope;

    public RegistrationViewModel()
    {
    }

    public RegistrationViewModel(ILifetimeScope scope)
    {
        _scope = scope;
    }


    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.",
        MinimumLength = 1)]
    [Display(Name = "FullName")]
    public string FullName { get; set; } 


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


    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
    public required string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }

    public void Act()
    {
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
    }
}