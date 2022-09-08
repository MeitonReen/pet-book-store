using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.NotFound.Configurators;

namespace BookStore.Base.Implementations.Result.Builders.NotFound.Builders
{
    public class NotFoundResultBuilderRoot : FluentApiBase
    {
        public NotFoundResultBuilderRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public NotFoundResultConfiguratorRoot Configurator => new(_objectResolver);
    }
}