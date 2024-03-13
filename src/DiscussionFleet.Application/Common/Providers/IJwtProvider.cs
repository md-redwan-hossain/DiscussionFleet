namespace DiscussionFleet.Application.Common.Providers;

public interface IJwtProvider
{
    string GenerateJwt(Dictionary<string, object> claims);
}