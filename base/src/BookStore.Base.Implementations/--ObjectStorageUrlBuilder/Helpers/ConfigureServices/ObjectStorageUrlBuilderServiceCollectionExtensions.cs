using BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Abstractions;
using BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Implementations;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers.ConfigureServices
{
    public static class ObjectStorageUrlBuilderServiceCollectionExtensions
    {
        public static void AddObjectStorageUrlBuilder(this IServiceCollection services) =>
            services.AddScoped<IObjectStorageUrlBuilder, ObjectStorageUrlBuilderRoot>();
    }
}