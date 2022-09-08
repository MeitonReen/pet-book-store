using Microsoft.AspNetCore.Builder;

namespace BookStore.Base.Implementations.UserClaimsProfile.Middleware.Default.Extensions
{
    public static class UserClaimsProfileExtensions
    {
        public static IApplicationBuilder UseUserClaimsProfile(
            this IApplicationBuilder app)
        {
            if (app == default)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<UserClaimsProfileMiddleware>();
        }
    }
}