using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Conflict.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Builders.Conflict.Configurators.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.Conflict.Configurators
{
    public class ConflictResultConfiguratorRoot : FluentApiBase
    {
        public ConflictResultConfiguratorRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ConflictResultConfiguratorRfc7807 UseRfc7807
        {
            get
            {
                _objectResolver.Register(
                    new ConflictResultBuilderRfc7807(_objectResolver));
                return new ConflictResultConfiguratorRfc7807(_objectResolver);
            }
        }

        public ConflictResultConfiguratorRoot Environment(string environment)
        {
            _objectResolver.Register(new EnvironmentAccessor(environment));
            return this;
        }
    }
}