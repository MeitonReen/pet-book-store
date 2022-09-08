using BookStore.Base.Abstractions;

namespace BookStore.BookService.Contracts.BookCategory.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class BookCategory
    {
        public const string Build = $"{Base}/book-category/{Version}";

        public static class BookRef
        {
            public const string Build = $"{BookCategory.Build}/book-ref";
        }
    }
}