using BookStore.Base.DefaultSettings.ConfigurationManager.Helpers;
using BookStore.Base.NonAttachedExtensions;
using Microsoft.Extensions.Configuration;

namespace BookStore.Base.DefaultSettings.ConfigurationManager;

public static class ConfigurationManagerDefaultSettings
{
    public static Microsoft.Extensions.Configuration.ConfigurationManager
        ApplyDefaultSettings(this
                Microsoft.Extensions.Configuration.ConfigurationManager configurationManager,
            Type targetTypeToAppSettingsJson,
            ConfigurationBuilderSettings? configurationManagerSettings = default)
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (environment == default) throw new InvalidOperationException("ASPNETCORE_ENVIRONMENT not found");

        var reloadOnChange = configurationManager
            .GetValue("hostBuilder:reloadConfigOnChange", true);

        configurationManager.LoadDefaultSettings(
            targetTypeToAppSettingsJson, environment, reloadOnChange, configurationManagerSettings);

        var appSettingsFilePathFromRootDirectory =
            targetTypeToAppSettingsJson.GetAbsoluteFilePathFromNamespace();

        configurationManagerSettings?.Invoke(configurationManager, appSettingsFilePathFromRootDirectory);

        if (!string.IsNullOrEmpty(appSettingsFilePathFromRootDirectory))
        {
            appSettingsFilePathFromRootDirectory = $"{appSettingsFilePathFromRootDirectory}/";
        }

        configurationManager
            .AddJsonFile($"{appSettingsFilePathFromRootDirectory}appsettings.json",
                true, reloadOnChange)
            .AddJsonFile($"{appSettingsFilePathFromRootDirectory}appsettings.{environment}.json",
                true, reloadOnChange);

        if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
        {
            configurationManager.AddUserSecrets(targetTypeToAppSettingsJson.Assembly,
                true, reloadOnChange);
        }

        return configurationManager;
    }
}