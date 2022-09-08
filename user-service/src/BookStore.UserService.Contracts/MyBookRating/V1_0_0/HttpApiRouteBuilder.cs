using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.MyBookRating.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class MyBookRating
    {
        public const string Build = $"{Base}/my-book-rating/{Version}";
    }
}