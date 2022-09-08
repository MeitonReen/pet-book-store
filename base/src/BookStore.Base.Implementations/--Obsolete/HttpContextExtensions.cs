using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace BookStore.Base.Implementations.__Obsolete
{
    public static class HttpContextExtensions
    {
        public static bool TryGetUserId(this HttpContext httpContext, out Guid userId)
        {
            return Guid.TryParse(httpContext.User.Claims.SingleOrDefault(claim =>
                claim.Type is ClaimTypes.NameIdentifier or OpenIddictConstants.Claims.Subject
            )?.Value, out userId);
        }

        public static string? GetUserName(this HttpContext httpContext)
        {
            return httpContext.User.Claims.SingleOrDefault(claim =>
                claim.Type is ClaimTypes.Name or OpenIddictConstants.Claims.Name)?.Value;
        }

        public static Guid? GetUserId(this HttpContext httpContext) =>
            httpContext.TryGetUserId(out var userId) ? userId : default;

        public static bool CheckContainedUserIdOnEquality(this HttpContext httpContext,
            Guid userId)
        {
            var userIdFromHttpContext = httpContext.GetUserId();
            return userIdFromHttpContext == userId;
        }
    }
}