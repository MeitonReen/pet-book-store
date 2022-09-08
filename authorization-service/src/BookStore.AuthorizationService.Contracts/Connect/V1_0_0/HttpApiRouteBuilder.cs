using BookStore.Base.Abstractions;

namespace BookStore.AuthorizationService.Contracts.Connect.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class Connect
    {
        public const string Build = $"{Base}/connect/{Version}";

        public static class Authorization
        {
            public const string Build = $"{Connect.Build}/authorization";
        }

        public static class Token
        {
            public const string Build = $"{Connect.Build}/token";
        }
    }
}