using System.IdentityModel.Tokens.Jwt;
using BookStore.AuthorizationService.Configs.DefaultClients;
using BookStore.AuthorizationService.Configs.DefaultUsers;
using BookStore.AuthorizationService.Settings.DefaultAppSettings;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.DefaultConfigs.Swagger;
using BookStore.Base.DefaultSettings.DiContainer;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultLogging.ActionMethods.Extensions;
using BookStore.Base.Implementations.CorrelationId.Extensions;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using BookStore.Base.Implementations.UserClaimsProfile.Contracts.Profile.Implementations.Default.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.AuthorizationService.Settings.DiContainer;

public static class RootDiSettings
{
    public static Task<IServiceCollection> AddDiSettings(
        this IServiceCollection services, IConfiguration configuration)
    {
        #region Load configs

        var defaultClientsConfig = configuration.Get<DefaultClientsConfig>(conf =>
        {
            conf.CasesSupport.AddUpperSnakeCase();
            conf.IncludeRootConfigClassAsConfigProperty = true;
        });
        ;
        services.Configure<DefaultClientsConfig>(configuration, conf =>
        {
            conf.CasesSupport.AddUpperSnakeCase();
            conf.IncludeRootConfigClassAsConfigProperty = true;
        });
        services.Configure<DefaultUsersConfig>(configuration, conf =>
        {
            conf.CasesSupport.AddUpperSnakeCase();
            conf.IncludeRootConfigClassAsConfigProperty = true;
        });

        var authorizationServiceConfig = configuration
            .Get<AuthorizationServiceConfig>(sets =>
            {
                sets.CasesSupport.AddUpperSnakeCase();
                sets.IncludeRootConfigClassAsConfigProperty = true;
            });
        var swaggerConfig = configuration.Get<SwaggerConfig>(sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        #endregion

        var validatorsAssembly = typeof(TargetAssemblyType).Assembly;
        var webControllersAssembly =
            typeof(WebEntryPoint.Helpers.TargetAssemblyType).Assembly;

        var xmlDocName = $"{webControllersAssembly.GetName().Name}.xml";
        var xmlDocPath = Path
            .TrimEndingDirectorySeparator(webControllersAssembly.Location
                .Replace(Path.GetFileName(webControllersAssembly.Location), ""));

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services
            .AddCorsPolicySettings(defaultClientsConfig)
            .AddIdentityDatabaseSettings(configuration)
            .AddQuartzSettings()
            .AddOpenIdDictSettings(authorizationServiceConfig)
            // .AddDefaultResourcesTools()
            .AddDefaultSwaggerSettings(authorizationServiceConfig, swaggerConfig, xmlDocName,
                xmlDocPath, typeof(Contracts.Helpers.TargetAssemblyType).Assembly)
            .AddDefaultFluentValidationSettings(validatorsAssembly)
            .AddHttpCorrelationId()
            .AddDefaultUserClaimsProfile()
            .AddEndpointsApiExplorer()
            .AddControllers(sets => sets
                .UseDateOnlyTimeOnlyStringConverters()) //Temp DateOnly support
            .AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters()) //Temp DateOnly support
            .AddApplicationPart(webControllersAssembly)
            .AddBookStoreDefaultExceptionHandling()
            .AddBookStoreDefaultLogging()
            .Services
            .AddSpaStaticFiles(sets => sets.RootPath = "./wwwroot");

        return Task.FromResult(services);
    }
}