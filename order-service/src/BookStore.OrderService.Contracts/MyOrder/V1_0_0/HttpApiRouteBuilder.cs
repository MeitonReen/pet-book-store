using BookStore.Base.Abstractions;

namespace BookStore.OrderService.Contracts.MyOrder.V1_0_0
{
    public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
    {
        public const string Version = "v1.0.0";

        public static class MyOrder
        {
            public const string Build = $"{Base}/my-order/{Version}";
        }
    }
}