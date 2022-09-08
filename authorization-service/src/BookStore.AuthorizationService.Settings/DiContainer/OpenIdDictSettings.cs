using System.Reflection;
using BookStore.AuthorizationService.Contracts.Connect.V1_0_0;
using BookStore.AuthorizationService.Data.BaseDatabase;
using BookStore.AuthorizationService.Defaults;
using BookStore.Base.DefaultConfigs;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace BookStore.AuthorizationService.Settings.DiContainer;

public static class OpenIdDictSettings
{
    public static IServiceCollection AddOpenIdDictSettings(this IServiceCollection services,
        AuthorizationServiceConfig config)
        => services
            .AddOpenIddict()
            .AddCore(sets =>
            {
                sets.UseEntityFrameworkCore()
                    .UseDbContext<BaseDbContext>();
                sets.UseQuartz();
            })
            .AddServer(sets =>
            {
                sets.SetIssuer(new Uri(config.Issuer));
                
                sets.SetAuthorizationEndpointUris($"/{HttpApiRouteBuilder.Connect.Authorization.Build}")
                    .SetTokenEndpointUris($"/{HttpApiRouteBuilder.Connect.Token.Build}");
                sets.RegisterScopes(
                    typeof(BookStoreDefaultScopes)
                        .GetFields(BindingFlags.Public | BindingFlags.Static)
                        .Select(el => el.GetValue(default) as string ??
                                      throw new InvalidOperationException("Specified scope not found"))
                        .Append(OpenIddictConstants.Scopes.OpenId)
                        .Append(OpenIddictConstants.Scopes.Profile)
                        .ToArray() ?? throw new InvalidOperationException("Specified scopes not found"));

                sets.AllowAuthorizationCodeFlow();
                sets.AddDevelopmentEncryptionCertificate()
                    .AddDevelopmentSigningCertificate();

                sets.UseAspNetCore()
                    .DisableTransportSecurityRequirement()
                    .EnableAuthorizationEndpointPassthrough()
                    .EnableTokenEndpointPassthrough();

                sets.DisableAccessTokenEncryption();
                sets.SetAccessTokenLifetime(TimeSpan.FromMinutes(60));
            })
            .AddValidation(sets =>
            {
                sets.UseLocalServer();
                sets.UseAspNetCore();
            })
            .Services;
}