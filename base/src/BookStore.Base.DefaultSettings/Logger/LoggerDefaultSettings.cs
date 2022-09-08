using BookStore.Base.DefaultConfigs;
using BookStore.Base.DefaultSettings.Logger.Serilog;
using BookStore.Base.Implementations.ExtendedConfigurationBinder;
using BookStore.Base.Implementations.ExtendedConfigurationBinder.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace BookStore.Base.DefaultSettings.Logger;

public static class LoggerDefaultSettings
{
    public static IConfiguration ApplyDefaultLoggerSettings(this
            IConfiguration configuration,
        ConfigureHostBuilder hostBuilderSettings,
        Action<LoggerConfiguration>? loggerSettings = default
    )
    {
        var logConfig = configuration.Get<LogConfig>(sets =>
        {
            sets.CasesSupport.AddUpperSnakeCase();
            sets.IncludeRootConfigClassAsConfigProperty = true;
        });

        Log.Logger = SerilogDefaultLoggerConfigurator.Configure(logConfig, loggerSettings);

        hostBuilderSettings.UseSerilog();

        return configuration;
    }

    public static void CloseAndFlush() => Log.CloseAndFlush();
}