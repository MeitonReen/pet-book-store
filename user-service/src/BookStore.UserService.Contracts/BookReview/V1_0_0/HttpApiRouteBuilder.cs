using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.BookReview.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class BookReview
    {
        public const string Build = $"{Base}/book-review/{Version}";
    }
}