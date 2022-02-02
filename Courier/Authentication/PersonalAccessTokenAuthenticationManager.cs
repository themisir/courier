using System.Security.Claims;
using System.Security.Principal;
using Microsoft.AspNetCore.Authentication;

namespace Courier.Authentication;

public class PersonalAccessTokenAuthenticationManager
{
    private readonly IPersonalAccessTokenStore _tokenStore;
    private readonly ISystemClock _clock;

    public PersonalAccessTokenAuthenticationManager(IPersonalAccessTokenStore tokenStore, ISystemClock clock)
    {
        _tokenStore = tokenStore;
        _clock = clock;
    }

    public async Task<AuthenticateResult> TryAuthenticate(string token, string scheme)
    {
        var personalToken = await _tokenStore.FindByToken(token);
        if (personalToken == null)
        {
            return AuthenticateResult.Fail("User not found");
        }

        if (personalToken.ExpiresAt != null && personalToken.ExpiresAt < _clock.UtcNow)
        {
            return AuthenticateResult.Fail("Token is expired");
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, personalToken.UserId),
        };
        var identity = new ClaimsIdentity(claims, scheme);
        var principal = new GenericPrincipal(identity, null);
        var ticket = new AuthenticationTicket(principal, scheme);
        return AuthenticateResult.Success(ticket);
    }
}