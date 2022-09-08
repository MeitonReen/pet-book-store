using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Exception.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Builders.Exception.Configurators.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.Exception.Configurators
{
    public class ExceptionResultConfiguratorRoot : FluentApiBase
    {
        public ExceptionResultConfiguratorRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ExceptionResultConfiguratorRfc7807 UseRfc7807
        {
            get
            {
                _objectResolver.Register(
                    new ExceptionResultBuilderRfc7807(_objectResolver));
                return new ExceptionResultConfiguratorRfc7807(_objectResolver);
            }
        }

        public ExceptionResultConfiguratorRoot Environment(string environment)
        {
            _objectResolver.Register(new EnvironmentAccessor(environment));
            return this;
        }
    }
}