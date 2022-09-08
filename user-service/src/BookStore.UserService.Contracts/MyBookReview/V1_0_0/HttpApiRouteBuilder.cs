using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.MyBookReview.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class MyBookReview
    {
        public const string Build = $"{Base}/my-book-review/{Version}";
    }
}