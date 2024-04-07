using System.Text.Json.Serialization;

namespace DiscussionFleet.Application.Common.Providers;

public class EmailTokenRateLimiter : ITokenRateLimiter
{
    public uint TotalTokenIssued { get; }
    public DateTime NextTokenAtUtc { get; }

    [JsonConstructor]
    private EmailTokenRateLimiter(uint totalTokenIssued, DateTime nextTokenAtUtc)
    {
        TotalTokenIssued = totalTokenIssued;
        NextTokenAtUtc = nextTokenAtUtc;
    }

    public EmailTokenRateLimiter(DateTime? tokenIssueTimeUtc = null, ushort intervalMinute = 60)
    {
        if (NextTokenAtUtc == default && tokenIssueTimeUtc.HasValue)
        {
            NextTokenAtUtc = tokenIssueTimeUtc.Value;
        }

        TotalTokenIssued += 1;
        var minute = TotalTokenIssued * intervalMinute;
        NextTokenAtUtc = NextTokenAtUtc.AddMinutes(minute);
    }
};