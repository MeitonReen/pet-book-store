using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.MyBookRatings.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class BookRatings
    {
        public const string Build = $"{Base}/book-ratings/{Version}";
    }
}