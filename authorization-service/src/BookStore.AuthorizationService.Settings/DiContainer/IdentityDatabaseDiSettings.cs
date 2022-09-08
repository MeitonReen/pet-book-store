using BookStore.AuthorizationService.BL.ResourceEntities;
using BookStore.AuthorizationService.Data.BaseDatabase;
using BookStore.AuthorizationService.Data.BaseDatabase.DatabaseInit;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.DefaultConfigs;
using BookStore.Base.Implementations.DatabaseInit;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace BookStore.AuthorizationService.Settings.DiContainer;

public static class IdentityDatabaseDiSettings
{
    public static IServiceCollection AddIdentityDatabaseSettings(
        this IServiceCollection services, IConfiguration configuration)
        => services
            .Configure<BaseDatabaseConfig>(configuration, sets =>
            {
                sets.CasesSupport.AddUpperSnakeCase();
                sets.IncludeRootConfigClassAsConfigProperty = true;
            })
            .AddDbContext<BaseDbContext>((serviceProvider, sets) => sets
                .UseNpgsql(serviceProvider
                    .GetRequiredService<IOptionsSnapshotMixOptionsMonitor<BaseDatabaseConfig>>()
                    .Value
                    .ConnectionString)
                .UseOpenIddict())
            .AddScoped<DbContext>(serviceProvider => serviceProvider
                .GetRequiredService<BaseDbContext>())
            .AddIdentity<User, IdentityRole<Guid>>(sets =>
            {
                sets.Password.RequireDigit = false;
                sets.Password.RequireNonAlphanumeric = false;
                sets.Password.RequireUppercase = false;
            })
            .AddEntityFrameworkStores<BaseDbContext>()
            .AddDefaultTokenProviders()
            .Services
            .AddTransient<IDatabaseInit, DatabaseInit>()
            .Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIddictConstants.Claims.Name;
                options.ClaimsIdentity.UserIdClaimType = OpenIddictConstants.Claims.Subject;
                options.ClaimsIdentity.RoleClaimType = OpenIddictConstants.Claims.Role;
                options.ClaimsIdentity.EmailClaimType = OpenIddictConstants.Claims.Email;

                options.SignIn.RequireConfirmedAccount = false;
            });
}