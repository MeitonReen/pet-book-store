using BookStore.Base.Abstractions;

namespace BookStore.UserService.Contracts.MyProfile.V1_0_0;

public class HttpApiRouteBuilder : HttpApiRouteBuilderBase
{
    public const string Version = "v1.0.0";

    public static class MyProfile
    {
        public const string Build = $"{Base}/my-profile/{Version}";
    }
}