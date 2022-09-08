using BookStore.Base.Abstractions;

namespace BookStore.BookService.Contracts.Authors.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class Authors
    {
        public const string Build = $"{Base}/authors/{Version}";
    }
}