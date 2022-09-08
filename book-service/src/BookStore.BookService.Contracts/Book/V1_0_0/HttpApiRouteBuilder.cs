using BookStore.Base.Abstractions;

namespace BookStore.BookService.Contracts.Book.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class Book
    {
        public const string Build = $"{Base}/book/{Version}";
    }
}