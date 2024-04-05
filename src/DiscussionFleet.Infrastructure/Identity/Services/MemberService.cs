using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Identity.Managers;
using Mapster;
using Microsoft.AspNetCore.Identity;
using SharpOutcome;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public class MemberService : IMemberService
{
    private readonly IApplicationUnitOfWork _applicationUnitOfWork;
    private readonly ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;

    public MemberService(IApplicationUnitOfWork applicationUnitOfWork, ApplicationUserManager userManager,
        ApplicationSignInManager signInManager, IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider)
    {
        _applicationUnitOfWork = applicationUnitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
    }

    public async Task<Outcome<Member, IMembershipError>> CreateAsync(MemberRegistrationRequest dto)
    {
        if (await _userManager.FindByNameAsync(dto.Email) is not null)
        {
            return new MembershipError(BadOutcomeTag.Conflict);
        }

        await using var trx = await _applicationUnitOfWork.BeginTransactionAsync();
        try
        {
            var applicationUser = new ApplicationUser
            {
                Id = _guidProvider.SortableGuid(),
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _userManager.CreateAsync(applicationUser, dto.Password);
            if (result.Succeeded is false)
            {
                List<KeyValuePair<string, string>> errors = [];

                errors.AddRange(result.Errors.Select(err =>
                    new KeyValuePair<string, string>(err.Code, err.Description)));

                await trx.RollbackAsync();
                return new MembershipError(BadOutcomeTag.Failure, errors);
            }

            var member = new Member { Id = applicationUser.Id, FullName = dto.FullName };
            member.SetCreatedAt(_dateTimeProvider.CurrentUtcTime);

            await _applicationUnitOfWork.MemberRepository.CreateAsync(member);
            await _applicationUnitOfWork.SaveAsync();

            await trx.CommitAsync();
            return member;
        }
        catch (Exception)
        {
            await trx.RollbackAsync();
        }

        return new MembershipError(BadOutcomeTag.Unknown);
    }
}