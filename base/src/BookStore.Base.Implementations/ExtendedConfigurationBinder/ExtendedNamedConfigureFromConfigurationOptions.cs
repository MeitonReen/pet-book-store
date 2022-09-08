using System.Diagnostics.CodeAnalysis;
using BookStore.Base.Abstractions.ClassDynamicWrapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace BookStore.Base.Implementations.ExtendedConfigurationBinder;

/// <summary>
/// Configures an option instance by using <see cref="ConfigurationBinder.Bind(IConfiguration, object)"/> against an <see cref="IConfiguration"/>.
/// </summary>
/// <typeparam name="TOptions">The type of options to bind.</typeparam>
public class ExtendedNamedConfigureFromConfigurationOptions<
    [DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)]
    TOptions> : ConfigureNamedOptions<TOptions>
    where TOptions : class
{
    /// <summary>
    /// Constructor that takes the <see cref="IConfiguration"/> instance to bind against.
    /// </summary>
    /// <param name="name">The name of the options instance.</param>
    /// <param name="config">The <see cref="IConfiguration"/> instance.</param>
    /// <param name="configureBinder">Used to configure the <see cref="BinderOptions"/>.</param>
    /// <param name="wrapperObjectToPropertyWrapper"></param>
    public ExtendedNamedConfigureFromConfigurationOptions(string name,
        IConfiguration config,
        Action<ExtendedBinderOptions> configureBinder,
        IWrapperObjectToPropertyWrapper wrapperObjectToPropertyWrapper)
        : base(name, options => BindFromOptions(options, config, configureBinder,
            wrapperObjectToPropertyWrapper))
    {
        if (config == default)
        {
            throw new ArgumentNullException(nameof(config));
        }
    }

    [UnconditionalSuppressMessage("ReflectionAnalysis", "IL2026:RequiresUnreferencedCode",
        Justification =
            "The only call to this method is the constructor which is already annotated as RequiresUnreferencedCode.")]
    private static void BindFromOptions(TOptions options, IConfiguration config,
        Action<ExtendedBinderOptions> configureBinderOptions,
        IWrapperObjectToPropertyWrapper wrapperObjectToPropertyWrapper)
    {
        object targetOptions = options;

        var binderOptions = new ExtendedBinderOptions();
        configureBinderOptions?.Invoke(binderOptions);

        if (binderOptions.IncludeRootConfigClassAsConfigProperty)
        {
            wrapperObjectToPropertyWrapper = wrapperObjectToPropertyWrapper ??
                                             throw new ArgumentNullException(
                                                 nameof(wrapperObjectToPropertyWrapper));
            targetOptions = wrapperObjectToPropertyWrapper.Wrap(options);
        }

        config.Bind(targetOptions, binderOptions);
    }
}