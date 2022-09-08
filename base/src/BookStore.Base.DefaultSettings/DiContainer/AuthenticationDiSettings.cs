using System.IdentityModel.Tokens.Jwt;
using BookStore.Base.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Validation.AspNetCore;

namespace BookStore.Base.DefaultSettings.DiContainer;

public static class AuthenticationDiSettings
{
    public static IServiceCollection AddDefaultAuthenticationSettings(
        this IServiceCollection services,
        AuthorizationServiceConfig authorizationServiceConfig)
    {
        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddOpenIddict()
            .AddValidation(sets =>
            {
                sets.SetIssuer(authorizationServiceConfig.Issuer);
                sets.UseSystemNetHttp();
                sets.UseAspNetCore();
            })
            .Services
            .AddAuthentication(OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme);
        return services;
    }
}