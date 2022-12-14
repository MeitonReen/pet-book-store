using BookStore.Base.Abstractions;

namespace BookStore.OrderService.Contracts.MyCart.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class MyCart
    {
        public const string Build = $"{Base}/my-cart/{Version}";

        public static class Book
        {
            public const string Build = $"{MyCart.Build}/book";
        }
    }
}