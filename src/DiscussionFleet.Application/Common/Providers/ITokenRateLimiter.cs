namespace DiscussionFleet.Application.Common.Providers;

public interface ITokenRateLimiter
{
    public uint TotalTokenIssued { get;  }

    public DateTime NextTokenAtUtc { get; }
}