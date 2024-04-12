using DiscussionFleet.Application;
using DiscussionFleet.Application.Common.Providers;
using DiscussionFleet.Application.Common.Services;
using DiscussionFleet.Application.Common.Utils;
using DiscussionFleet.Application.MembershipFeatures;
using DiscussionFleet.Contracts.Membership;
using DiscussionFleet.Domain.Entities;
using DiscussionFleet.Domain.Outcomes;
using DiscussionFleet.Infrastructure.Identity.Managers;
using Hangfire;
using Mapster;
using Microsoft.AspNetCore.Identity;
using SharpOutcome;
using StackExchange.Redis;

namespace DiscussionFleet.Infrastructure.Identity.Services;

public class MemberService : IMemberService
{
    private readonly IApplicationUnitOfWork _appUnitOfWork;
    private readonly ApplicationUserManager _userManager;
    private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
    private readonly IEmailService _emailService;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IGuidProvider _guidProvider;
    private readonly IConnectionMultiplexer _redis;
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IJsonSerializationProvider _jsonSerializationProvider;
    private readonly IFileBucketService _fileBucketService;


    public MemberService(IApplicationUnitOfWork appUnitOfWork, ApplicationUserManager userManager,
        IDateTimeProvider dateTimeProvider, IGuidProvider guidProvider, IEmailService emailService,
        IConnectionMultiplexer redis, IJsonSerializationProvider jsonSerializationProvider,
        IBackgroundJobClient backgroundJobClient, IPasswordHasher<ApplicationUser> passwordHasher,
        IFileBucketService fileBucketService)
    {
        _appUnitOfWork = appUnitOfWork;
        _userManager = userManager;
        _dateTimeProvider = dateTimeProvider;
        _guidProvider = guidProvider;
        _emailService = emailService;
        _redis = redis;
        _jsonSerializationProvider = jsonSerializationProvider;
        _backgroundJobClient = backgroundJobClient;
        _passwordHasher = passwordHasher;
        _fileBucketService = fileBucketService;
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

    #region Update Member Profile

    public async Task<MemberProfileUpdateResult> UpdateAsync(MemberUpdateRequest dto, Guid id)
    {
        var memberInDb = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == id);
        if (memberInDb is null) return MemberProfileUpdateResult.EntityNotFound;

        var member = await dto.BuildAdapter().AdaptToAsync(memberInDb);
        await _appUnitOfWork.MemberRepository.UpdateAsync(member);
        await _appUnitOfWork.SaveAsync();
        return MemberProfileUpdateResult.Ok;
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
                            <h2>Welcome, {member.FullName}!</h2>
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


    public async Task<bool> HasCorrectCredentialsAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user?.PasswordHash is null) return false;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is PasswordVerificationResult.Success;
    }

    public async Task<bool> CanRequestEmailConfirmationAsync(string userName, string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user?.PasswordHash is null) return false;

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
        return result is not PasswordVerificationResult.Failed && user.EmailConfirmed is false;
    }

    public async Task<Outcome<ApplicationUser, CredentialError>> RequestEmailConfirmationAsync(string userName,
        string password)
    {
        var user = await _userManager.FindByNameAsync(userName);
        if (user?.PasswordHash is null)
        {
            return CredentialError.UserNameNotFound;
        }

        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result is PasswordVerificationResult.Failed)
        {
            return CredentialError.PasswordNotMatched;
        }

        if (user.EmailConfirmed)
        {
            return CredentialError.ProfileAlreadyConfirmed;
        }

        return user;
    }


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

    public async Task<bool> CacheMemberInfoAsync(string id, MemberCachedInformation memberInfo)
    {
        var cache = _redis.GetDatabase();
        var json = _jsonSerializationProvider.Serialize(memberInfo);
        return await cache.HashSetAsync(RedisConstants.MemberInformationHashStore, id, json).ConfigureAwait(false);
    }

    public async Task<bool> FlushMemberInfoCacheAsync(string id)
    {
        var cache = _redis.GetDatabase();
        return await cache.HashDeleteAsync(RedisConstants.MemberInformationHashStore, id);
    }


    public async Task<MemberCachedInformation?> RefreshMemberInfoCacheAsync(string id)
    {
        var entity = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == Guid.Parse(id),
            subsetSelector: x => new { x.Id, x.FullName, x.ProfileImageId });
        if (entity is null) return null;
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return null;


        string? imgName = null;
        if (entity.ProfileImageId is not null)
        {
            var img = await _appUnitOfWork
                .MultimediaImageRepository
                .GetOneAsync(x => x.Id == entity.ProfileImageId);

            imgName = img?.ImageNameResolver();
        }

        var info = new MemberCachedInformation(entity.FullName, user.EmailConfirmed, imgName,
            await CacheImageUrl(imgName), _dateTimeProvider.CurrentUtcTime.AddHours(1));

        await CacheMemberInfoAsync(id, info);
        return info;
    }


    public async Task<bool> UpdateMemberProfileUrlCacheAsync(string id, uint ttlInMinute = 60)
    {
        var data = await GetCachedMemberInfoAsync(id);
        if (data is null) return false;
        if (data.ProfileImageName is not null)
        {
            var url = await _fileBucketService.GetImageUrlAsync(data.ProfileImageName);
            var updatedMemberData = data with
            {
                ProfileImageUrl = url,
                ProfileImageUrlExpirationUtc = _dateTimeProvider.CurrentUtcTime.AddMinutes(ttlInMinute)
            };
            await CacheMemberInfoAsync(id, updatedMemberData);
        }

        return true;
    }

    public async Task<MemberCachedInformation?> GetCachedMemberInfoAsync(string id)
    {
        var cache = _redis.GetDatabase();
        var json = await cache.HashGetAsync(RedisConstants.MemberInformationHashStore, id).ConfigureAwait(false);
        if (json.IsNullOrEmpty) return null;
        return _jsonSerializationProvider.DeSerialize<MemberCachedInformation>(json.ToString());
    }


    public async Task<Outcome<Success, IResendEmailError>> ResendEmailVerificationTokenAsync(ApplicationUser user)
    {
        var cachedData = await GetCachedEmailVerifyHistoryAsync(user.Id.ToString());

        if (cachedData is not null && CanResendToken(cachedData.NextTokenAtUtc) is false)
        {
            return new ResendEmailError(ResendEmailErrorReason.TooEarly, cachedData.NextTokenAtUtc);
        }

        return ProcessResendResult(await IssueEmailToken(user));
    }

    public async Task<Outcome<Success, IResendEmailError>> ResendEmailVerificationTokenAsync(string id)
    {
        var cachedData = await GetCachedEmailVerifyHistoryAsync(id);

        if (cachedData is not null && CanResendToken(cachedData.NextTokenAtUtc) is false)
        {
            return new ResendEmailError(ResendEmailErrorReason.TooEarly, cachedData.NextTokenAtUtc);
        }

        return ProcessResendResult(await IssueEmailToken(id));
    }


    private static Outcome<Success, IResendEmailError> ProcessResendResult(VerificationEmailResult result)
    {
        if (result is VerificationEmailResult.Ok) return new Success();
        return new ResendEmailError(ResendEmailErrorReason.EntityNotFound);
    }


    private bool CanResendToken(DateTime nextTokenAtUtc)
    {
        var result = _dateTimeProvider.CurrentUtcTime >= nextTokenAtUtc;
        return result;
    }

    private async Task<VerificationEmailResult> IssueEmailToken(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null) return VerificationEmailResult.EntityNotFound;
        return await IssueEmailToken(user).ConfigureAwait(false);
    }


    private async Task<VerificationEmailResult> IssueEmailToken(ApplicationUser user)
    {
        var member = await _appUnitOfWork.MemberRepository.GetOneAsync(x => x.Id == user.Id);
        if (member is null) return VerificationEmailResult.EntityNotFound;

        await _userManager.UpdateSecurityStampAsync(user);
        var token = await IssueVerificationMailTokenAsync(user);
        _backgroundJobClient.Enqueue(() => SendVerificationMailAsync(user, member, token));
        await UpsertEmailVerificationCache(user.Id.ToString());
        return VerificationEmailResult.Ok;
    }


    private async Task UpsertEmailVerificationCache(string id)
    {
        var tokenData = await GetCachedEmailVerifyHistoryAsync(id);
        if (tokenData is not null)
        {
            tokenData.UpdateToken();
            await CacheEmailVerifyHistoryAsync(id, tokenData).ConfigureAwait(false);
        }
        else
        {
            var tokenRateLimiter = new EmailTokenRateLimiter(_dateTimeProvider.CurrentUtcTime);
            await CacheEmailVerifyHistoryAsync(id, tokenRateLimiter).ConfigureAwait(false);
        }
    }

    private async Task<string?> CacheImageUrl(string? key)
    {
        if (key is null) return null;
        var imageUrl = await _fileBucketService.GetImageUrlAsync(key);
        return imageUrl;
    }
}