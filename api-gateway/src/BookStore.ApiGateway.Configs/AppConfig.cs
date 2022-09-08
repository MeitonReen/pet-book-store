namespace BookStore.ApiGateway.Configs
{
    public class AppConfig : Base.DefaultConfigs.AppConfig
    {
        public string ExternalUri { get; set; } = string.Empty;

        public string AuthorizationServiceUri { get; set; } = string.Empty;
        public string BookServiceUri { get; set; } = string.Empty;
        public string UserServiceUri { get; set; } = string.Empty;
        public string OrderServiceUri { get; set; } = string.Empty;
    }
}