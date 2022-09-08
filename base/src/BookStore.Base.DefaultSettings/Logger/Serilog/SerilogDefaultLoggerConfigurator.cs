using BookStore.Base.DefaultConfigs;
using Serilog;
using Serilog.Events;

namespace BookStore.Base.DefaultSettings.Logger.Serilog;

public static class SerilogDefaultLoggerConfigurator
{
    public static ILogger Configure(LogConfig logConfig,
        Action<LoggerConfiguration>? loggerSettings = default)
    {
        var loggerConfig = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
            .MinimumLevel.Override("System", LogEventLevel.Warning)
            .Enrich.FromLogContext()
            .WriteTo.Console(theme: logConfig.ConsoleTheme,
                outputTemplate: logConfig.OutputTemplate)
            .WriteTo.File(logConfig.LogFilePath);

        loggerSettings?.Invoke(loggerConfig);

        return loggerConfig.CreateLogger();
    }
}