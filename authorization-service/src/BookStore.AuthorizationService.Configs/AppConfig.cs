namespace BookStore.AuthorizationService.Configs;

public class AppConfig : Base.DefaultConfigs.AppConfig
{
    public string SpaDevelopmentServerUri { get; init; } = string.Empty;
    public static AppConfig Empty => new();
}