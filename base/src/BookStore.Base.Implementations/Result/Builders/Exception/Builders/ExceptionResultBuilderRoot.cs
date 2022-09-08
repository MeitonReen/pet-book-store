using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Exception.Configurators;

namespace BookStore.Base.Implementations.Result.Builders.Exception.Builders
{
    public class ExceptionResultBuilderRoot : FluentApiBase
    {
        public ExceptionResultBuilderRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ExceptionResultConfiguratorRoot Configurator =>
            _objectResolver.Resolve<ExceptionResultConfiguratorRoot>() ??
            new ExceptionResultConfiguratorRoot(_objectResolver);
    }
}