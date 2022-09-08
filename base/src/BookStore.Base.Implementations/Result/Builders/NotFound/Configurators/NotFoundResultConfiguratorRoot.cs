using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.NotFound.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Builders.NotFound.Configurators.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.NotFound.Configurators
{
    public class NotFoundResultConfiguratorRoot : FluentApiBase
    {
        public NotFoundResultConfiguratorRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public NotFoundResultConfiguratorRfc7807 UseRfc7807
        {
            get
            {
                _objectResolver.Register(
                    new NotFoundResultBuilderRfc7807(_objectResolver));
                return new NotFoundResultConfiguratorRfc7807(_objectResolver);
            }
        }

        public NotFoundResultConfiguratorRoot Environment(string environment)
        {
            _objectResolver.Register(new EnvironmentAccessor(environment));
            return this;
        }
    }
}