using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;
using SharpOutcome;

namespace DiscussionFleet.Web.Models.Account;

public class ConfirmAccountViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IMemberIdentityService _memberIdentityService;
    private ApplicationUserManager _userManager;
    [Required] public string Code { get; set; }
    [Required] public Guid UserId { get; set; }
    public bool HasError { get; set; }

    public ConfirmAccountViewModel()
    {
    }

    public ConfirmAccountViewModel(ILifetimeScope scope, IMemberIdentityService memberIdentityService,
        ApplicationUserManager userManager)
    {
        _scope = scope;
        _memberIdentityService = memberIdentityService;
        _userManager = userManager;
    }

    public async Task<Outcome<ApplicationUser, IBadOutcome>> ConductConfirmationAsync(string id, string code)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return new BadOutcome(BadOutcomeTag.NotFound, "User not found");

        if (user.EmailConfirmed) return new BadOutcome(BadOutcomeTag.Repetitive, "Already verified.");

        var result = await _memberIdentityService.ConfirmEmailAsync(user, code);
        if (result is false) return new BadOutcome(BadOutcomeTag.Invalid, "Invalid verification code.");

        return user;
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _memberIdentityService = _scope.Resolve<IMemberIdentityService>();
        _userManager = _scope.Resolve<ApplicationUserManager>();
    }
}