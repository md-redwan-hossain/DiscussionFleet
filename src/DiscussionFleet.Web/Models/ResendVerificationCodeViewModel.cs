using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using SharpOutcome;

namespace DiscussionFleet.Web.Models;

public class ResendVerificationCodeViewModel : IViewModelWithResolve
{

    private ILifetimeScope _scope;
    private IMemberService _memberService;


    public ResendVerificationCodeViewModel()
    {

    }

    public ResendVerificationCodeViewModel(ILifetimeScope scope, IMemberService memberService)
    {
        _scope = scope;
        _memberService = memberService;
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
        var userOutcome = await _memberService.RequestEmailConfirmationAsync(Email, Password);

        if (userOutcome.TryPickGoodOutcome(out var user))
        {
            var outcome = await _memberService.ResendEmailVerificationTokenAsync(user);

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
        _memberService = _scope.Resolve<IMemberService>();
    }
}