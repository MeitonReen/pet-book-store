using Serilog.Sinks.SystemConsole.Themes;

namespace BookStore.Base.DefaultConfigs;

public class LogConfig
{
    public string LogFilePath { get; init; } = string.Empty;
    public string OutputTemplate { get; init; } = string.Empty;
    public AnsiConsoleTheme ConsoleTheme { get; init; } = AnsiConsoleTheme.Code;
}