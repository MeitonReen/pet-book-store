using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.DefaultSettings.DiContainer;

public static class DatabaseDiSettings
{
    // public static IServiceCollection AddDefaultBaseDatabaseSettings<
    //     TDataContext, TIDatabaseInitImplementation>(
    //     this IServiceCollection services,
    //     IConfiguration configuration,
    //     BaseDatabaseConfig? baseDatabaseConfig = default)
    //     where TIDatabaseInitImplementation : class, IDatabaseInit
    //     where TDataContext : DbContext
    // {
    //     if (baseDatabaseConfig != default)
    //     {
    //         services
    //             .AddDbContext<TDataContext>(sets => sets
    //                 .UseNpgsql(baseDatabaseConfig.ConnectionString));
    //     }
    //     else
    //     {
    //         services
    //             .Configure<BaseDatabaseConfig>(configuration, sets =>
    //             {
    //                 sets.CasesSupport.AddUpperSnakeCase();
    //                 sets.IncludeRootConfigClassAsConfigProperty = true;
    //             })
    //             .AddDbContext<TDataContext>((serviceProvider, sets) =>
    //                 sets.UseNpgsql(serviceProvider
    //                     .GetRequiredService<
    //                         IOptionsSnapshotMixOptionsMonitor<BaseDatabaseConfig>>()
    //                     .Value
    //                     .ConnectionString));
    //     }
    //
    //     return services
    //         .AddScoped<DbContext>(serviceProvider => serviceProvider
    //             .GetRequiredService<TDataContext>())
    //         .AddTransient<IDatabaseInit, TIDatabaseInitImplementation>();
    // }

    public static IServiceCollection AddDefaultDatabaseSettings<
        TDataContext, TIDatabaseInitImplementation, TDatabaseConfig>(
        this IServiceCollection services,
        IConfiguration configuration,
        TDatabaseConfig? databaseConfig = default,
        Func<string, string>? connectionStringModificator = default)
        where TDatabaseConfig : BaseDatabaseConfig
        where TDataContext : DbContext
        where TIDatabaseInitImplementation : class, IDatabaseInit
    {
        if (databaseConfig != default)
        {
            services
                .AddDbContext<TDataContext>(sets => sets
                    .UseNpgsql(connectionStringModificator
                                   ?.Invoke(databaseConfig.ConnectionString) ??
                               databaseConfig.ConnectionString));
        }
        else
        {
            services
                .Configure<TDatabaseConfig>(configuration, sets =>
                {
                    sets.CasesSupport.AddUpperSnakeCase();
                    sets.IncludeRootConfigClassAsConfigProperty = true;
                })
                .AddDbContext<TDataContext>((serviceProvider, sets) =>
                {
                    var targetConnectionString = serviceProvider
                        .GetRequiredService<
                            IOptionsSnapshotMixOptionsMonitor<TDatabaseConfig>>()
                        .Value
                        .ConnectionString;

                    targetConnectionString =
                        connectionStringModificator
                            ?.Invoke(targetConnectionString) ?? targetConnectionString;

                    sets.UseNpgsql(targetConnectionString);
                });
        }

        return services.AddTransient<IDatabaseInit, TIDatabaseInitImplementation>();
    }

    public static IServiceCollection AddNewDatabasePerScopeSettings<
        TDataContext, TIDatabaseInitImplementation, TDatabaseConfig>(
        this IServiceCollection services,
        IConfiguration configuration,
        TDatabaseConfig? databaseConfig = default,
        string? databaseNamePrefix = default)
        where TDatabaseConfig : BaseDatabaseConfig
        where TDataContext : DbContext
        where TIDatabaseInitImplementation : class, IDatabaseInit
        => services.AddDefaultDatabaseSettings<TDataContext, TIDatabaseInitImplementation,
            TDatabaseConfig>(configuration, databaseConfig, connectionString =>
        {
            const string databaseToken = "Database=";

            var indexBetweenDatabaseSegment = connectionString
                .IndexOf(databaseToken, StringComparison.Ordinal);
            var indexAfterDatabaseSegment = connectionString
                .IndexOf(';', indexBetweenDatabaseSegment);

            return
                connectionString[..indexBetweenDatabaseSegment] +
                $"{databaseToken}{databaseNamePrefix ?? "TestDb_"}{Guid.NewGuid()}" +
                connectionString[indexAfterDatabaseSegment..];
        });
}