using BookStore.Base.DefaultConfigs;
using BookStore.Base.DefaultConfigs.Swagger;
using BookStore.Base.DefaultSettings.DiContainer;
using BookStore.Base.Implementations.BookStoreDefaultExceptionHandling.ActionMethods.Extensions;
using BookStore.Base.Implementations.BookStoreDefaultLogging.ActionMethods.Extensions;
using BookStore.Base.Implementations.CorrelationId.Extensions;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using BookStore.Base.Implementations.ResourcesAuthorization.Extensions;
using BookStore.Base.Implementations.UserClaimsProfile.Contracts.Profile.Implementations.Default.Extensions;
using BookStore.BookService.Configs;
using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.Data.BaseDatabase.DatabaseInit;
using BookStore.BookService.Data.SagasDatabase;
using BookStore.BookService.Settings.DefaultAppSettings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BaseDatabaseConfig = BookStore.BookService.Configs.BaseDatabaseConfig;

namespace BookStore.BookService.Settings.DiContainer;

public static class RootDiSettings
{
    public static async Task<IServiceCollection> AddDiSettings(
        this IServiceCollection services, IConfiguration configuration)
    {
        #region Load configs

        services.Configure<AppConfig>(configuration, sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        var masstransitConfig = configuration
            .Get<MasstransitConfig>(sets =>
            {
                sets.CasesSupport.AddUpperSnakeCase();
                sets.IncludeRootConfigClassAsConfigProperty = true;
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

        var mapperAssembly = typeof(TargetAssemblyType).Assembly;
        var validatorsAssembly = mapperAssembly;
        var webControllersAssembly = typeof(WebEntryPoint.Helpers.TargetAssemblyType).Assembly;

        var xmlDocName = $"{webControllersAssembly.GetName().Name}.xml";
        var xmlDocPath = Path
            .TrimEndingDirectorySeparator(webControllersAssembly.Location
                .Replace(Path.GetFileName(webControllersAssembly.Location), ""));

        services
            .AddDefaultDatabaseSettings<SagasDbContext, Data.SagasDatabase.DatabaseInit.DatabaseInitRuntime,
                SagasDatabaseConfig>(configuration)
            .AddDefaultResourcesTools<BaseDbContext, DatabaseInitRuntime,
                BaseDatabaseConfig>(configuration)
            .AddBlResources()
            .AddDefaultSwaggerSettings(authorizationServiceConfig, swaggerConfig, xmlDocName,
                xmlDocPath, typeof(Contracts.Helpers.TargetAssemblyType).Assembly)
            .AddAutoMapper(mapperAssembly)
            .AddDefaultFluentValidationSettings(validatorsAssembly)
            .AddMassTransitSettings(masstransitConfig)
            .AddHttpCorrelationId()
            .AddDefaultUserClaimsProfile()
            .AddHealthChecks()
            .Services
            .AddEndpointsApiExplorer()
            .AddControllers(sets => sets
                .UseDateOnlyTimeOnlyStringConverters()) //Temp DateOnly support
            .AddJsonOptions(options => options.UseDateOnlyTimeOnlyStringConverters()) //Temp DateOnly support
            .AddApplicationPart(webControllersAssembly)
            .AddBookStoreDefaultExceptionHandling()
            .AddBookStoreDefaultLogging()
            .Services
            .AddDefaultAuthenticationSettings(authorizationServiceConfig);

        await services.AddResourcesAuthorization(webControllersAssembly);
        return services;
    }
}