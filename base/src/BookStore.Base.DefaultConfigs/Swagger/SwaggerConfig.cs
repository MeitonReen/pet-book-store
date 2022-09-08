namespace BookStore.Base.DefaultConfigs.Swagger;

public class SwaggerConfig
{
    public SwaggerUiApplicationClientConfig Ui { get; init; } =
        SwaggerUiApplicationClientConfig.Empty;

    public OpenApiDocConfig[] OpenApiDocs { get; set; } = Array.Empty<OpenApiDocConfig>();
}