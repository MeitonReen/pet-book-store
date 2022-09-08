using BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Abstractions;
using BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers.Configurator;
using BookStore.Base.Implementations.FluentApi;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Implementations
{
    public class ObjectStorageUrlBuilderRoot : FluentApiBase, IObjectStorageUrlBuilder
    {
        public ObjectStorageUrlBuilderRoot(IHttpContextAccessor httpContextAccessor,
            ILogger<ObjectStorageUrlBuilderRoot> logger) : base(ObjectResolverFirst)
        {
            _objectResolver.Register(httpContextAccessor);
            _objectResolver.Register<ILogger>(logger);
        }

        public ObjectStorageUrlConfigurator Configurator
        {
            get => new(_objectResolver);
        }
    }
}