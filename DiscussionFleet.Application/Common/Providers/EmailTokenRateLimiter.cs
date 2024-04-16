using System.Text.Json.Serialization;

namespace DiscussionFleet.Application.Common.Providers;

public class EmailTokenRateLimiter : ITokenRateLimiter
{
    public uint TotalTokenIssued { get; private set; }
    public DateTime NextTokenAtUtc { get; private set; }
    private readonly ushort _intervalMinute;

    [JsonConstructor]
    private EmailTokenRateLimiter(uint totalTokenIssued, DateTime nextTokenAtUtc)
    {
        TotalTokenIssued = totalTokenIssued;
        NextTokenAtUtc = nextTokenAtUtc;
    }

    public EmailTokenRateLimiter(DateTime? tokenIssueTimeUtc = null, ushort intervalMinute = 60)
    {
        _intervalMinute = intervalMinute;
        if (NextTokenAtUtc == default && tokenIssueTimeUtc.HasValue)
        {
            NextTokenAtUtc = tokenIssueTimeUtc.Value;
        }

        UpdateToken(intervalMinute);
    }

    public void UpdateToken(ushort intervalMinute = 60)
    {
        TotalTokenIssued += 1;
        var minute = TotalTokenIssued * _intervalMinute;
        NextTokenAtUtc = NextTokenAtUtc.AddMinutes(minute);
    }
};