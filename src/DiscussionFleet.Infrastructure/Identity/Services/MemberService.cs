using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Identity.Managers;
using Mapster;
using Microsoft.AspNetCore.Identity;
using SharpOutcome;
using StackExchange.Redis;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public class MemberService : IMemberService
{
    private readonly IApplicationUnitOfWork _applicationUnitOfWork;
    private readonly ApplicationUserManager _userManager;
    private readonly ApplicationSignInManager _signInManager;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;
    private readonly IConnectionMultiplexer _redis;
    private readonly IJsonSerializationProvider _jsonSerializationProvider;


    public MemberService(IApplicationUnitOfWork applicationUnitOfWork, ApplicationUserManager userManager,
        ApplicationSignInManager signInManager, IDateTimeProvider dateTimeProvider,
        IGuidProvider guidProvider, IEmailService emailService,
        IConnectionMultiplexer redis, IJsonSerializationProvider jsonSerializationProvider)
    {
        _applicationUnitOfWork = applicationUnitOfWork;
        _userManager = userManager;
        _signInManager = signInManager;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _emailService = emailService;
        _redis = redis;
        _jsonSerializationProvider = jsonSerializationProvider;
    }

    #region Create A Member

    public async Task<Outcome<(ApplicationUser applicationUser, Member member), IMembershipError>> CreateAsync(
        MemberRegistrationRequest dto)
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
            return (applicationUser, member);
        }
        catch (Exception)
        {
            await trx.RollbackAsync();
        }

        return new MembershipError(BadOutcomeTag.Unknown);
    }

    #endregion

    #region Issue Verification Code in Email

    public Task<string> IssueVerificationMailTokenAsync(ApplicationUser user)
    {
        return _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    #endregion

    #region Send Verification Email

    public async Task SendVerificationMailAsync(ApplicationUser applicationUser, Member member,
        string verificationCode)
    {
        ArgumentNullException.ThrowIfNull(applicationUser.Email);
        const string subject = "Account Confirmation";

        var body = $"""
                    <html>
                        <body>
                            <h1>Welcome, {member.FullName}!</h1>
                            <p>Thanks for signing up. Please verify your email address by using the following verification code:</p>
                            <h2>{verificationCode}</h2>
                            <p>If you didn't request this, you can safely ignore this email.</p>
                            <p>Best Regards,</p>
                            <p>DiscussionFleet</p>
                        </body>
                    </html>
                    """;

        await _emailService
            .SendSingleEmailAsync(member.FullName, applicationUser.Email, subject, body)
            .ConfigureAwait(false);
    }

    #endregion

    public async Task SaveVerificationEmailHistoryAsync(string id, VerificationEmailHistory verificationEmailHistory)
    {
        var cache = _redis.GetDatabase();
        var json = _jsonSerializationProvider.Serialize(verificationEmailHistory);
        await cache.HashSetAsync(RedisConstants.EmailHistoryHashStore, id, json).ConfigureAwait(false);
    }

    public async Task<bool> ConfirmEmailAsync(ApplicationUser applicationUser, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(applicationUser, token).ConfigureAwait(false);
        return result.Succeeded;
    }
}