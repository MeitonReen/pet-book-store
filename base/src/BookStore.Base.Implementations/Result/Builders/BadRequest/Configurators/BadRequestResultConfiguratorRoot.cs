using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Configurators.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.BadRequest.Configurators
{
    public class BadRequestResultConfiguratorRoot : FluentApiBase
    {
        public BadRequestResultConfiguratorRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public BadRequestResultConfiguratorRfc7807 UseRfc7807
        {
            get
            {
                _objectResolver.Register(
                    new BadRequestResultBuilderRfc7807(_objectResolver));
                return new BadRequestResultConfiguratorRfc7807(_objectResolver);
            }
        }

        public BadRequestResultConfiguratorRoot Environment(string environment)
        {
            _objectResolver.Register(new EnvironmentAccessor(environment));
            return this;
        }
    }
}