using BookStore.Base.DefaultConfigs;
using BookStore.Base.DefaultSettings.DiContainer;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using BookStore.OrderService.Data.BaseDatabase;
using BookStore.OrderService.IntegrationTests.Data.DatabaseInit;
using BookStore.OrderService.Settings.DefaultAppSettings;
using BookStore.OrderService.Settings.DiContainer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.OrderService.IntegrationTests.Settings.DiContainer;

public static class RootDiSettings
{
    public static IServiceCollection AddDiSettings(this IServiceCollection services,
        IConfiguration configuration)
    {
        #region Load configs

        services.Configure<AppConfig>(configuration, sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        var masstransitConfig = configuration.Get<MasstransitConfig>(sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        #endregion

        var mapperAssembly = typeof(TargetAssemblyType).Assembly;
        var validatorsAssembly = mapperAssembly;

        services
            .AddMassTransitSettings(masstransitConfig)
            .AddUserClaimsProfileStubSettings()
            .AddBlResources()
            .AddDefaultResourcesTools<BaseDbContext>(dbSets =>
                dbSets.AddNewDatabasePerScopeSettings<BaseDbContext, DatabaseInitRuntime,
                    BaseDatabaseConfig>(configuration))
            .AddAutoMapper(mapperAssembly)
            .AddDefaultFluentValidationSettings(validatorsAssembly);

        return services;
    }
    // services.AddDbContext<DataContext>(sets =>
    //     sets.UseInMemoryDatabase(Guid.NewGuid().ToString()));
    //
    // services.AddScoped<IDatabaseInit, DatabaseInitTestRuntime>();
    //
    // services.AddScoped<IUnitOfWork, UnitOfWork>();
    //
    // // services.AddScoped<IAuthorService, AuthorService>();
    // // services.AddScoped<IBookCategoryService, BookCategoryService>();
    // // services.AddScoped<IBookService, BL.Implementations.BookService>();
    //
    // services.AddHttpCorrelationId();
    //
    // services.AddAutoMapper(typeof(AuthorsMap).Assembly);
    //
}