using BookStore.Base.DefaultConfigs;
using BookStore.Base.DefaultConfigs.Swagger;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MMLib.SwaggerForOcelot.Configuration;
using Ocelot.DependencyInjection;

namespace BookStore.ApiGateway.Settings.DiContainer;

public static class RootDiSettings
{
    public static IServiceCollection AddDiSettings(
        this IServiceCollection services, IConfiguration configuration)
    {
        #region Load configs

        services.Configure<AppConfig>(configuration, sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        var swaggerUiConfig = configuration.Get<SwaggerUiApplicationClientConfig>(sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        #endregion

        return services
            .AddEndpointsApiExplorer()
            .AddOcelot()
            .Services
            .AddSwaggerForOcelot(configuration)
            .AddSwaggerGen()
            .Configure<SwaggerForOcelotUIOptions>(sets =>
            {
                sets.OAuthClientId(swaggerUiConfig.ClientId);
                sets.OAuthClientSecret(swaggerUiConfig.ClientSecret);
                // Array.ForEach(swaggerConfig.OpenApiDocs, el =>
                //     sets.SwaggerEndpoint($"/swagger/{el.Version}/swagger.json", el.Name));
                //conf.UseRequestInterceptor("(req) => { if (req.url.endsWith('connect/token') && req.body) req.body += '&audience=" + Constants.Authentication.AudienceSelector.BookStore.BookServiceApi.Select + "'; return req; }");
            });
    }
}