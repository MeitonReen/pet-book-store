using BookStore.Base.Abstractions;

namespace BookStore.BookService.Contracts.Books.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class Books
    {
        public const string Build = $"{Base}/books/{Version}";

        public static class AppliedFilters
        {
            public const string Build = $"{Books.Build}/applied-filters";
        }
    }
}