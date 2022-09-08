using System.Diagnostics.CodeAnalysis;
using BookStore.Base.Abstractions.ClassDynamicWrapper;
using BookStore.Base.Abstractions.OptionsScopedChanges;
using BookStore.Base.Implementations.ClassDynamicWrapper;
using BookStore.Base.Implementations.OptionsScopedChanges;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;

public static class ExtendedOptionsConfigurationServiceCollectionExtensions
{
    /// <summary>
    /// Registers a configuration instance which TOptions will bind against.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="config">The configuration being bound.</param>
    /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection
        Configure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(
            this IServiceCollection services, IConfiguration config, Action<ExtendedBinderOptions> configureBinder)
        where TOptions : class
        => services.Configure<TOptions>(Options.DefaultName, config, configureBinder);

    /// <summary>
    /// Registers a configuration instance which TOptions will bind against.
    /// </summary>
    /// <typeparam name="TOptions">The type of options being configured.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="config">The configuration being bound.</param>
    /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
    /// <returns>The <see cref="IServiceCollection"/> so that additional calls can be chained.</returns>
    public static IServiceCollection
        Configure<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] TOptions>(
            this IServiceCollection services, string name, IConfiguration config,
            Action<ExtendedBinderOptions> configureBinder)
        where TOptions : class
    {
        if (services == null)
        {
            throw new ArgumentNullException(nameof(services));
        }

        if (config == null)
        {
            throw new ArgumentNullException(nameof(config));
        }

        services.TryAddSingleton<IWrapperObjectToPropertyWrapper,
            WrapperObjectToPropertyWrapper>();

        services.AddScoped(
            typeof(IOptionsSnapshotMixOptionsMonitor<>),
            typeof(OptionsSnapshotMixOptionsMonitor<>));

        services.AddOptions();
        services.AddSingleton<IOptionsChangeTokenSource<TOptions>>(
            new ConfigurationChangeTokenSource<TOptions>(name, config));

        return services.AddSingleton<IConfigureOptions<TOptions>>(serviceProvider =>
            new ExtendedNamedConfigureFromConfigurationOptions<TOptions>(
                name, config, configureBinder,
                serviceProvider.GetRequiredService<IWrapperObjectToPropertyWrapper>()));
    }
}