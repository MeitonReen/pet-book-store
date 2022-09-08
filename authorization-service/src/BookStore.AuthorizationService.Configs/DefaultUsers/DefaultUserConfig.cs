namespace BookStore.AuthorizationService.Configs.DefaultUsers;

public class DefaultUserConfig
{
    public readonly string[] AllowedScopes = Array.Empty<string>();
    public readonly string Name = string.Empty;

    public DefaultUserConfig()
    {
    }

    public DefaultUserConfig(string name, string[] allowedScopes)
    {
        Name = name;
        AllowedScopes = allowedScopes;
    }

    public string Password { get; init; } = string.Empty;

    public static DefaultUserConfig Empty => new();
}