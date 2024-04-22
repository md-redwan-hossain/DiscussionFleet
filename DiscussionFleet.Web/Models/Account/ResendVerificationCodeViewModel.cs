using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application.MembershipFeatures.Enums;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using SharpOutcome;

namespace DiscussionFleet.Web.Models.Account;

public class ResendVerificationCodeViewModel : IViewModelWithResolve
{

    private ILifetimeScope _scope;
    private IMemberIdentityService _memberIdentityService;


    public ResendVerificationCodeViewModel()
    {

    }

    public ResendVerificationCodeViewModel(ILifetimeScope scope, IMemberIdentityService memberIdentityService)
    {
        _scope = scope;
        _memberIdentityService = memberIdentityService;
    }


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



    public async Task<Outcome<Guid, string>> ConductResendVerificationCode()
    {
        var userOutcome = await _memberIdentityService.RequestEmailConfirmationAsync(Email, Password);

        if (userOutcome.TryPickGoodOutcome(out var user))
        {
            var outcome = await _memberIdentityService.ResendEmailVerificationTokenAsync(user);

            if (outcome.IsGoodOutcome)
            {
                return user.Id;
            }

            if (outcome.TryPickBadOutcome(out var err))
            {
                if (err.Reason is ResendEmailErrorReason.TooEarly)
                {
                    return $"Wait until {err.NextTokenAtUtc} UTC";
                }
            }
        }
        return "Invalid attempt.";
    }


    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _memberIdentityService = _scope.Resolve<IMemberIdentityService>();
    }
}