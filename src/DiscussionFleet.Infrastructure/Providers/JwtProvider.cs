using System.Text;
using DiscussionFleet.Application.Common.Options;
using DiscussionFleet.Application.Common.Providers;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace DiscussionFleet.Infrastructure.Providers;

public class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _jwtOptions;
    private readonly TimeProvider _timeProvider;

    public JwtProvider(IOptions<JwtOptions> jwtOptions, TimeProvider timeProvider)
    {
        _timeProvider = timeProvider;
        _jwtOptions = jwtOptions.Value;
    }

    public string GenerateJwt(Dictionary<string, object> claims)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var descriptor = new SecurityTokenDescriptor
        {
            Claims = claims,
            IssuedAt = _timeProvider.GetUtcNow().UtcDateTime,
            Expires = _timeProvider.GetUtcNow().UtcDateTime.AddMinutes(_jwtOptions.ExpiryMinutes),
            SigningCredentials = credentials
        };

        var handler = new JsonWebTokenHandler { SetDefaultTimesOnTokenCreation = false };
        return handler.CreateToken(descriptor);
    }
}