using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Conflict.Configurators;

namespace BookStore.Base.Implementations.Result.Builders.Conflict.Builders
{
    public class ConflictResultBuilderRoot : FluentApiBase
    {
        public ConflictResultBuilderRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ConflictResultConfiguratorRoot Configurator => new(_objectResolver);
    }
}