using BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions;
using BookStore.Base.Implementations.CorrelationId.Accessor.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.CorrelationId.Accessor.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddCorrelationIdAccessor(
            this IServiceCollection services)
            => services.AddScoped<ICorrelationIdAccessor, CorrelationIdAccessor>();
    }
}