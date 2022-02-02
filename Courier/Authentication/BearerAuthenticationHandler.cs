using System.Net.Http.Headers;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace Courier.Authentication;

public class BearerAuthenticationHandler : AuthenticationHandler<BearerAuthenticationOptions>
{
    public const string HeaderScheme = "Bearer";
    
    private readonly PersonalAccessTokenAuthenticationManager _authenticationManager;
    
    public BearerAuthenticationHandler(IOptionsMonitor<BearerAuthenticationOptions> options, ILoggerFactory logger,
        UrlEncoder encoder, ISystemClock clock, PersonalAccessTokenAuthenticationManager authenticationManager)
        : base(options, logger, encoder, clock)
    {
        _authenticationManager = authenticationManager;
    }

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (AuthenticationHeaderValue.TryParse(Request.Headers.Authorization, out var headerValue) &&
            headerValue.Scheme.Equals(HeaderScheme, StringComparison.OrdinalIgnoreCase) &&
            headerValue.Parameter != null)
        {
            return await _authenticationManager.TryAuthenticate(headerValue.Parameter,
                Scheme.Name);
        }

        return AuthenticateResult.Fail("Not authenticated");
    }
}