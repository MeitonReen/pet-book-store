using BookStore.Base.Abstractions.UserClaimsProfile.Contracts;
using BookStore.Base.Abstractions.UserClaimsProfile.Contracts.Profile;
using Microsoft.AspNetCore.Http;
using OpenIddict.Abstractions;

namespace BookStore.Base.Implementations.UserClaimsProfile.Middleware.Default
{
    public class UserClaimsProfileMiddleware
    {
        private readonly RequestDelegate _next;

        public UserClaimsProfileMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task Invoke(HttpContext context, IUserClaimsProfileSetter userClaimsProfileSetter)
        {
            userClaimsProfileSetter.UserId = context.User.Claims
                .FirstOrDefault(claim => claim.Type is OpenIddictConstants.Claims.Subject)
                ?.Value ?? ProfileConstants.Anonymous;

            userClaimsProfileSetter.Name = context.User.Claims
                .FirstOrDefault(claim => claim.Type is OpenIddictConstants.Claims.Name)
                ?.Value ?? $"{OpenIddictConstants.Claims.Name} not found";

            return _next(context);
        }
    }
}