using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Infrastructure.Identity.Managers;
using Hangfire;
using SharpOutcome;
using StackExchange.Redis;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public class MemberService : IMemberService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly ApplicationUserManager _userManager;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;
    private readonly IConnectionMultiplexer _redis;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IJsonSerializationProvider _jsonSerializationProvider;


    public MemberService(IApplicationUnitOfWork appUnitOfWork, ApplicationUserManager userManager,
        IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider, IEmailService emailService,
        IConnectionMultiplexer redis, IJsonSerializationProvider jsonSerializationProvider,
        IBackgroundJobClient backgroundJobClient)
    {
        _appUnitOfWork = appUnitOfWork;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _emailService = emailService;
        _redis = redis;
        _jsonSerializationProvider = jsonSerializationProvider;
        _backgroundJobClient = backgroundJobClient;
    }

    #region Create A Member

    public async Task<Outcome<(ApplicationUser applicationUser, Member member), IMembershipError>> CreateAsync(
        MemberRegistrationRequest dto)
    {
        await using var trx = await _appUnitOfWork.BeginTransactionAsync();
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
                var duplicateEmail = result.Errors.FirstOrDefault(error => error.Code == "DuplicateEmail");

                if (duplicateEmail is null)
                {
                    errors.AddRange(result.Errors.Select(err =>
                        new KeyValuePair<string, string>(err.Code, err.Description)));
                }

                else
                {
                    errors.Add(new KeyValuePair<string, string>(duplicateEmail.Code, duplicateEmail.Description));
                }

                await trx.RollbackAsync();
                return new MembershipError(BadOutcomeTag.Failure, errors);
            }

            var member = new Member { Id = applicationUser.Id, FullName = dto.FullName };
            member.SetCreatedAt(_dateTimeProvider.CurrentUtcTime);

            await _appUnitOfWork.MemberRepository.CreateAsync(member);
            await _appUnitOfWork.SaveAsync();

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

    public async Task CacheEmailVerifyHistoryAsync(string id, ITokenRateLimiter rateLimiter)
    {
        var cache = _redis.GetDatabase();
        var json = _jsonSerializationProvider.Serialize(rateLimiter);
        await cache.HashSetAsync(RedisConstants.VerifyEmailHashStore, id, json).ConfigureAwait(false);
    }

    public async Task<ITokenRateLimiter?> GetCachedEmailVerifyHistoryAsync(string id)
    {
        var cache = _redis.GetDatabase();
        var json = await cache.HashGetAsync(RedisConstants.VerifyEmailHashStore, id).ConfigureAwait(false);
        if (json.IsNullOrEmpty) return null;
        return _jsonSerializationProvider.DeSerialize<EmailTokenRateLimiter>(json.ToString());
    }

    public async Task<bool> ConfirmEmailAsync(ApplicationUser user, string token)
    {
        var result = await _userManager.ConfirmEmailAsync(user, token).ConfigureAwait(false);
        if (result.Succeeded is false) return false;

        var cache = _redis.GetDatabase();
        await cache.HashDeleteAsync(RedisConstants.VerifyEmailHashStore, user.Id.ToString()).ConfigureAwait(false);
        return true;
    }

    public async Task CacheMemberInfoAsync(string id, MemberCachedInformation memberInfo)
    {
        var cache = _redis.GetDatabase();
        var json = _jsonSerializationProvider.Serialize(memberInfo);
        await cache.HashSetAsync(RedisConstants.MemberInformationHashStore, id, json).ConfigureAwait(false);
    }

    public async Task<MemberCachedInformation?> GetCachedMemberInfoAsync(string id)
    {
        var cache = _redis.GetDatabase();
        var json = await cache.HashGetAsync(RedisConstants.MemberInformationHashStore, id).ConfigureAwait(false);
        if (json.IsNullOrEmpty) return null;
        return _jsonSerializationProvider.DeSerialize<MemberCachedInformation>(json.ToString());
    }


    private Outcome<bool, IResendEmailError> ProcessEmailTokenResult( Outcome<bool,IBadOutcome> result)
    {
        if (result.TryPickBadOutcome(out var err))
        {
            return new ResendEmailError(err.Tag);
        }

        return true;
    }

    public async Task<Outcome<bool, IResendEmailError>> ResendEmailVerificationToken(ApplicationUser user)
    {
        var cachedData = await GetCachedEmailVerifyHistoryAsync(user.Id.ToString());

        if (cachedData is not null && IsValidTokenResendRequest(cachedData.NextTokenAtUtc) is false)
        {
            return new ResendEmailError(BadOutcomeTag.Rejected, cachedData.NextTokenAtUtc);
        }

        var result = await IssueEmailToken(user);
        return ProcessEmailTokenResult(result);
    }

    public async Task<Outcome<bool, IResendEmailError>> ResendEmailVerificationToken(string id)
    {
        var cachedData = await GetCachedEmailVerifyHistoryAsync(id);

        if (cachedData is not null && IsValidTokenResendRequest(cachedData.NextTokenAtUtc) is false)
        {
            return new ResendEmailError(BadOutcomeTag.Rejected, cachedData.NextTokenAtUtc);
        }

        var result = await IssueEmailToken(id);
        return ProcessEmailTokenResult(result);
    }
    
    // public async Task<Outcome<bool, IResendEmailError>> ResendEmailVerificationToken(ApplicationUser user)
    // {
    //     var cachedData = await GetCachedEmailVerifyHistoryAsync(user.Id.ToString());
    //
    //     if (cachedData is not null && IsValidTokenResendRequest(cachedData.NextTokenAtUtc) is false)
    //     {
    //         return new ResendEmailError(BadOutcomeTag.Rejected, cachedData.NextTokenAtUtc);
    //     }
    //
    //     var result = await IssueEmailToken(user);
    //
    //     if (result.TryPickBadOutcome(out var err))
    //     {
    //         return new ResendEmailError(err.Tag);
    //     }
    //
    //     return true;
    // }
    //
    //
    // public async Task<Outcome<bool, IResendEmailError>> ResendEmailVerificationToken(string id)
    // {
    //     var cachedData = await GetCachedEmailVerifyHistoryAsync(id);
    //
    //     if (cachedData is not null && IsValidTokenResendRequest(cachedData.NextTokenAtUtc) is false)
    //     {
    //         return new ResendEmailError(BadOutcomeTag.Rejected, cachedData.NextTokenAtUtc);
    //     }
    //
    //     var result = await IssueEmailToken(id);
    //
    //     if (result.TryPickBadOutcome(out var err))
    //     {
    //         return new ResendEmailError(err.Tag);
    //     }
    //
    //     return true;
    // }


    private bool IsValidTokenResendRequest(DateTime nextTokenAtUtc) =>
        nextTokenAtUtc <= _dateTimeProvider.CurrentUtcTime;

    private async Task<Outcome<bool, IBadOutcome>> IssueEmailToken(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return new BadOutcome(BadOutcomeTag.NotFound);
        return await IssueEmailToken(user).ConfigureAwait(false);
    }


    private async Task<Outcome<bool, IBadOutcome>> IssueEmailToken(ApplicationUser user)
    {
        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == user.Id);
        if (member is null) return new BadOutcome(BadOutcomeTag.NotFound);

        await _userManager.UpdateSecurityStampAsync(user);
        var token = await IssueVerificationMailTokenAsync(user);
        _backgroundJobClient.Enqueue(() => SendVerificationMailAsync(user, member, token));
        await UpsertEmailVerificationCache(user.Id.ToString());
        return true;
    }


    private async Task UpsertEmailVerificationCache(string id)
    {
        var tokenData = await GetCachedEmailVerifyHistoryAsync(id);
        if (tokenData is not null)
        {
            tokenData.UpdateToken();
            await CacheEmailVerifyHistoryAsync(id, tokenData);
        }
        else
        {
            var tokenRateLimiter = new EmailTokenRateLimiter(_dateTimeProvider.CurrentUtcTime);
            await CacheEmailVerifyHistoryAsync(id, tokenRateLimiter);
        }
    }
}