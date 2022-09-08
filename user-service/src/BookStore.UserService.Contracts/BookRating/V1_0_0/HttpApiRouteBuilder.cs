using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.BookRating.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class BookRating
    {
        public const string Build = $"{Base}/book-rating/{Version}";
    }
}