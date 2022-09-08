using BookStore.AuthorizationService.Configs.DefaultClients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.AuthorizationService.Settings.DiContainer;

public static class CorsPolicyDiSettings
{
    public static IServiceCollection AddCorsPolicySettings(
        this IServiceCollection services, DefaultClientsConfig defaultClientsConfig)
        => services
            .AddCors(sets => sets
                .AddDefaultPolicy(policySets => policySets
                    .WithOrigins(defaultClientsConfig.BookStoreSwaggerUiConfig.CorsOrigins)
                    .WithMethods(HttpMethods.Options, HttpMethods.Post)
                    .WithHeaders("x-requested-with")));
}