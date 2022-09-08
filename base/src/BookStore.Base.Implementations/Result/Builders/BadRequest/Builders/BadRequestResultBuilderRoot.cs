using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Configurators;

namespace BookStore.Base.Implementations.Result.Builders.BadRequest.Builders
{
    public class BadRequestResultBuilderRoot : FluentApiBase
    {
        public BadRequestResultBuilderRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public BadRequestResultConfiguratorRoot Configurator => new(_objectResolver);
    }
}