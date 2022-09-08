namespace BookStore.Base.DefaultConfigs;

public class AuthorizationServiceConfig
{
    public string Issuer { get; init; } = string.Empty;
    public string SignInUri { get; init; } = string.Empty;
    public string TokenUri { get; init; } = string.Empty;
}