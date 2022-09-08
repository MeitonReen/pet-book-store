using BookStore.Base.DefaultSettings.ConfigurationManager.Helpers;
using BookStore.Base.NonAttachedExtensions;
using Microsoft.Extensions.Configuration;

namespace BookStore.Base.DefaultSettings.ConfigurationManager;

public static class ConfigurationBuilderDefaultSettings
{
    public static IConfigurationBuilder LoadDefaultSettings(
        this IConfigurationBuilder configurationManager,
        Type targetTypeToAppSettingsJson,
        string environment,
        bool reloadOnChange,
        ConfigurationBuilderSettings? configurationManagerSettings = default)
    {
        if (environment == default) throw new ArgumentNullException(nameof(environment));

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