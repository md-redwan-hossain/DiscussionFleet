using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.MembershipFeatures.DataTransferObjects;
using DiscussionFleet.Application.MembershipFeatures.Interfaces;
using DiscussionFleet.Application.MembershipFeatures.Utils;
using DiscussionFleet.Infrastructure.Identity;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using SharpOutcome;

namespace DiscussionFleet.Web.Models.Account;

public class RegistrationViewModel : IViewModelWithResolve
{
    #region Fields

    private ILifetimeScope _scope;
    private ICloudQueueService _cloudQueueService;
    private IJsonSerializationService _jsonSerializationService;
    private IMemberIdentityService _memberIdentityService;
    private IDateTimeProvider _dateTimeProvider;

    #endregion

    #region Constructors

    public RegistrationViewModel()
    {
    }

    public RegistrationViewModel(ILifetimeScope scope, IMemberIdentityService memberIdentityService,
        IDateTimeProvider dateTimeProvider, ICloudQueueService cloudQueueService)
    {
        _scope = scope;
        _memberIdentityService = memberIdentityService;
        _dateTimeProvider = dateTimeProvider;
        _cloudQueueService = cloudQueueService;
    }

    #endregion

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
    public string ConfirmPassword { get; set; }

    public string? ReturnUrl { get; set; }
    public bool HasError { get; set; }

    #endregion

    #region Methods

    public async Task<Outcome<Guid, IMembershipError>> ConductRegistrationAsync()
    {
        var dto = new MemberRegistrationRequest(FullName, Email, Password);
        var result = await _memberIdentityService.CreateAsync(dto);
        if (result.TryPickBadOutcome(out var err)) return (MembershipError)err;

        result.TryPickGoodOutcome(out var userData);
        var token = await _memberIdentityService.IssueVerificationMailTokenAsync(userData.applicationUser);

        if (userData.applicationUser.Email is null)
        {
            return new MembershipError(BadOutcomeTag.NotFound, []);
        }

        var confirmationEmail = new MemberConfirmationEmail(
            userData.applicationUser.Id,
            userData.member.FullName,
            userData.applicationUser.Email,
            token
        );

        var json = _jsonSerializationService.Serialize(confirmationEmail);
        await _cloudQueueService.EnqueueAsync(json, userData.applicationUser.Id.ToString());

        var tokenRateLimiter = new EmailTokenRateLimiter(tokenIssueTimeUtc: _dateTimeProvider.CurrentUtcTime);
        await _memberIdentityService.CacheEmailVerifyHistoryAsync(userData.member.Id.ToString(), tokenRateLimiter);

        return userData.applicationUser.Id;
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _memberIdentityService = scope.Resolve<IMemberIdentityService>();
        _dateTimeProvider = _scope.Resolve<IDateTimeProvider>();
        _jsonSerializationService = _scope.Resolve<IJsonSerializationService>();
        _cloudQueueService = _scope.Resolve<ICloudQueueService>();
    }

    #endregion
}