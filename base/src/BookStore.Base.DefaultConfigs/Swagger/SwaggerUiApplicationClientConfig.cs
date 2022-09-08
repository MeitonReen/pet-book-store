namespace BookStore.Base.DefaultConfigs.Swagger;

public class SwaggerUiApplicationClientConfig
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
    public static SwaggerUiApplicationClientConfig Empty => new();
}