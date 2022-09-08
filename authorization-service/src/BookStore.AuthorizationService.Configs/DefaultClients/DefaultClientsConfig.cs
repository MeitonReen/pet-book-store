namespace BookStore.AuthorizationService.Configs.DefaultClients;

public class DefaultClientsConfig
{
    public BookStoreSwaggerUiConfig BookStoreSwaggerUiConfig { get; init; }
        = BookStoreSwaggerUiConfig.Empty;
}