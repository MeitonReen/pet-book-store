namespace BookStore.AuthorizationService.Configs.DefaultClients;

public class BookStoreSwaggerUiConfig
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public string[] RedirectUris { get; init; } = Array.Empty<string>();
    public string[] CorsOrigins { get; init; } = Array.Empty<string>();
    public string DisplayName { get; init; } = string.Empty;
    public string[] AllowedScopes { get; set; } = Array.Empty<string>();

    public static BookStoreSwaggerUiConfig Empty => new();
}