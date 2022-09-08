using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.MyBookReviews.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class BookReviews
    {
        public const string Build = $"{Base}/book-reviews/{Version}";
    }
}