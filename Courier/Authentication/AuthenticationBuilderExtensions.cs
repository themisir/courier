using Microsoft.AspNetCore.Authentication;

namespace Courier.Authentication;

public static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddBearerScheme<TTokenStore>(this AuthenticationBuilder builder)
        where TTokenStore : class, IPersonalAccessTokenStore
    {
        builder.Services
            .AddScoped<PersonalAccessTokenAuthenticationManager>()
            .AddScoped<IPersonalAccessTokenStore, TTokenStore>();
        
        return builder.AddScheme<BearerAuthenticationOptions, BearerAuthenticationHandler>("Bearer", null);
    }
}