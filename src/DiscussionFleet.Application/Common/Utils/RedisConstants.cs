using Microsoft.Extensions.Caching.Distributed;

namespace DiscussionFleet.Application.Common.Utils;

public static class RedisConstants
{
    public const string DataProtectionKeys = "DataProtectionKeys";
    public const string EmailHistoryHashStore = "email_history_hs";
    public const string MemberInformationHashStore = "member_info_hs";
    public const string Hangfire = "Hangfire";
    public const string StackExchangeInstance = "DiscussionFleet";

    public static DistributedCacheEntryOptions DefaultRedisOpts(
        TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null
    )
    {
        return new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
            SlidingExpiration = unusedExpireTime
        };
    }
}