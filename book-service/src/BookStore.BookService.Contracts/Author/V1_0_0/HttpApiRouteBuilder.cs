using BookStore.Base.Abstractions;

namespace BookStore.BookService.Contracts.Author.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class Author
    {
        public const string Build = $"{Base}/author/{Version}";
    }
}