using BookStore.Base.Implementations.CorrelationId.Accessor.Extensions;
using BookStore.Base.Implementations.CorrelationId.Configs;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.CorrelationId.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddHttpCorrelationId(
            this IServiceCollection services)
            => services.AddCorrelationIdAccessor();

        public static IServiceCollection AddHttpCorrelationId(
            this IServiceCollection services,
            Action<HttpCorrelationIdOptions> configure)
            => services
                .AddCorrelationIdAccessor()
                .Configure(configure);
    }
}