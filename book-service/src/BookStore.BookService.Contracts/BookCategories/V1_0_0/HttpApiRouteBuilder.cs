using BookStore.Base.Abstractions;

namespace BookStore.BookService.Contracts.BookCategories.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class BookCategories
    {
        public const string Build = $"{Base}/book-categories/{Version}";
    }
}