using BookStore.Base.Abstractions.BaseResources.Inner;
using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource;
using BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.BaseResources.Inner;
using BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit;
using BookStore.Base.Implementations.BaseResources.Outer.BaseResourceExistenceByMassTransit;
using BookStore.Base.Implementations.DatabaseInit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.DefaultSettings.DiContainer;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultResourcesTools<TDataContext>(
        this IServiceCollection services,
        Func<IServiceCollection, IServiceCollection> databaseSettings)
        where TDataContext : BaseBookStoreDbContext
        => databaseSettings(services)
            .AddDefaultResourcesToolsInner<TDataContext>();

    public static IServiceCollection AddDefaultResourcesTools<TDataContext,
        TIDatabaseInitImplementation, TDatabaseConfig>(
        this IServiceCollection services,
        IConfiguration configuration,
        TDatabaseConfig? databaseConfig = default,
        Func<string, string>? connectionStringModificator = default)
        where TDatabaseConfig : BaseDatabaseConfig
        where TDataContext : BaseBookStoreDbContext
        where TIDatabaseInitImplementation : class, IDatabaseInit
        => services
            .AddDefaultDatabaseSettings<TDataContext, TIDatabaseInitImplementation,
                TDatabaseConfig>(configuration, databaseConfig, connectionStringModificator)
            .AddDefaultResourcesToolsInner<TDataContext>();

    private static IServiceCollection AddDefaultResourcesToolsInner<TDataContext>(
        this IServiceCollection services)
        where TDataContext : BaseBookStoreDbContext
        => services
            .AddScoped<BaseBookStoreDbContext>(serviceProvider => serviceProvider
                .GetRequiredService<TDataContext>())
            .AddScoped(typeof(IBaseResource<>), typeof(BaseResourceByEfCore<>))
            .AddScoped(typeof(IBaseResourceCollection<>), typeof(BaseResourceCollectionByEfCore<>))
            .AddScoped(typeof(IBaseResourceCollectionCount<>), typeof(BaseResourceCollectionCountByEfCore<>))
            .AddScoped(typeof(IBaseResourceExistence<>), typeof(BaseResourceExistenceByEfCore<>))
            .AddScoped<IResourcesCommitter, EfCoreResourcesCommitter>()
            .AddScoped(typeof(IBaseOuterResource<>), typeof(BaseOuterResourceByMassTransit<>))
            .AddScoped<IBaseOuterResourceExistence, BaseOuterResourceExistenceByMassTransit>();
}