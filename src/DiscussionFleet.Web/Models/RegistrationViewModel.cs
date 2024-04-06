using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Infrastructure.Identity.Services;
using Hangfire;

namespace DiscussionFleet.Web.Models;

public class RegistrationViewModel
{
    private ILifetimeScope _scope;
    private IMemberService _memberService;
    private IEmailService _emailService;
    private IDateTimeProvider _dateTimeProvider;
    private IBackgroundJobClient _backgroundJobClient;


    public RegistrationViewModel()
    {
    }

    public RegistrationViewModel(ILifetimeScope scope, IMemberService memberService,
        IEmailService emailService, IDateTimeProvider dateTimeProvider,
        IBackgroundJobClient backgroundJobClient)
    {
        _scope = scope;
        _memberService = memberService;
        _emailService = emailService;
        _dateTimeProvider = dateTimeProvider;
        _backgroundJobClient = backgroundJobClient;
    }

    #region Properties

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

    #endregion

    public async Task<IMembershipError?> ConductRegistrationAsync()
    {
        var dto = new MemberRegistrationRequest(FullName, Email, Password);
        var result = await _memberService.CreateAsync(dto);
        if (result.TryPickBadOutcome(out var err)) return err;

        result.TryPickGoodOutcome(out var userData);
        var token = await _memberService.IssueVerificationMailTokenAsync(userData.applicationUser);

        _backgroundJobClient.Enqueue(() =>
            _memberService.SendVerificationMailAsync(userData.applicationUser, userData.member, token));


        // await _memberService.SendVerificationMailAsync(userData.applicationUser, userData.member, token);

        var history = new EmailHistory(token, _dateTimeProvider.CurrentLocalTime, _dateTimeProvider.CurrentLocalTime);
        await _memberService.SaveEmailHistoryAsync(userData.applicationUser.Id.ToString(), history);
        return null;
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _memberService = scope.Resolve<IMemberService>();
        _emailService = _scope.Resolve<IEmailService>();
        _dateTimeProvider = _scope.Resolve<IDateTimeProvider>();
        _backgroundJobClient = _scope.Resolve<IBackgroundJobClient>();
    }
}