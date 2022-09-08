using Microsoft.AspNetCore.Builder;

namespace BookStore.Base.Implementations.CorrelationId.Middleware.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        public static IApplicationBuilder UseHttpCorrelationId(
            this IApplicationBuilder app)
        {
            if (app == default)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.UseMiddleware<HttpCorrelationIdMiddleware>();
        }
    }
}