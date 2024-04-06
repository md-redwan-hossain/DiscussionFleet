using System.ComponentModel.DataAnnotations;
using Autofac;
using DiscussionFleet.Infrastructure.Identity.Managers;
using DiscussionFleet.Infrastructure.Identity.Services;
using DiscussionFleet.Web.Utils;

namespace DiscussionFleet.Web.Models;

public class ConfirmAccountViewModel : IViewModelWithResolve
{
    private ILifetimeScope _scope;
    private IMemberService _memberService;
    private ApplicationUserManager _userManager;
    [Required] public string Code { get; set; }

    [Required] public Guid UserId { get; set; }
    public uint TryAgainAfterSeconds { get; set; } = 5;

    public ConfirmAccountViewModel()
    {
    }

    public ConfirmAccountViewModel(ILifetimeScope scope, IMemberService memberService,
        ApplicationUserManager userManager)
    {
        _scope = scope;
        _memberService = memberService;
        _userManager = userManager;
    }

    public async Task<bool> ConductConfirmationAsync(string id, string code)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return false;

        var result = await _memberService.ConfirmEmailAsync(user, code);
        return result;
    }

    public void Resolve(ILifetimeScope scope)
    {
        _scope = scope;
        _memberService = scope.Resolve<IMemberService>();
        _userManager = scope.Resolve<ApplicationUserManager>();
    }
}