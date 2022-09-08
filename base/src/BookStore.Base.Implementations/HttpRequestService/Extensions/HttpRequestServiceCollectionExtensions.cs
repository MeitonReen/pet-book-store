using BookStore.Base.Implementations.HttpRequestService.Abstractions;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.HttpRequestService.Extensions
{
    public static class HttpRequestServiceCollectionExtensions
    {
        public static void AddHttpRequestService(this IServiceCollection services)
        {
            services.AddTransient<IHttpRequestService, HttpRequestService>();
        }
    }
}