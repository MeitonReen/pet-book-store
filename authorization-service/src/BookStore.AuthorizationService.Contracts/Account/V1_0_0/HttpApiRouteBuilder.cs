using BookStore.Base.Abstractions;

namespace BookStore.AuthorizationService.Contracts.Account.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class Account
    {
        public const string Build = $"{Base}/account/{Version}";
    }
}