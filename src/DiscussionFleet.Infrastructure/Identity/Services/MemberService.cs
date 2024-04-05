using DiscussionFleet.Application;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Identity.Managers;
using Microsoft.AspNetCore.Identity;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public class MemberService : IMemberService
{
    private readonly ApplicationUserManager _userManager;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly ApplicationSignInManager _signInManager;

    private readonly IApplicationUnitOfWork _applicationUnitOfWork;

    public MemberService(IApplicationUnitOfWork applicationUnitOfWork, ApplicationUserManager userManager,
        IPasswordHasher<ApplicationUser> passwordHasher, ApplicationSignInManager signInManager)
    {
        _applicationUnitOfWork = applicationUnitOfWork;
        _userManager = userManager;
        _passwordHasher = passwordHasher;
        _signInManager = signInManager;
    }

    public async Task<Outcome<Member, IBadOutcome>> CreateAsync(MemberRegistrationRequest dto, CancellationToken token)
    {
        if (await _userManager.FindByNameAsync(dto.Email) is not null)
        {
            return new BadOutcome(BadOutcomeTag.Conflict);
        }

        await using (var trx = _applicationUnitOfWork.BeginTransaction())
        {
        }

        throw new NotImplementedException();
    }
}