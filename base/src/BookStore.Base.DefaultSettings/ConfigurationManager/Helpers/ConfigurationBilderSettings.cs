using Microsoft.Extensions.Configuration;

namespace BookStore.Base.DefaultSettings.ConfigurationManager.Helpers;

public delegate void ConfigurationBuilderSettings(IConfigurationBuilder configManager,
    string appSettingsFilePathFromRootDirectory);