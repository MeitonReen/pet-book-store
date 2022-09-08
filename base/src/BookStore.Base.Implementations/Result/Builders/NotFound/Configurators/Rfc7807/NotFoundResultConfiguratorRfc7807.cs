using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.NotFound.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.NotFound.Configurators.Rfc7807
{
    public class NotFoundResultConfiguratorRfc7807 : FluentApiBase
    {
        private readonly ResultConfiguratorBaseRfc7807 _rfc7807ResultConfiguratorBase = new();

        public NotFoundResultConfiguratorRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
            _rfc7807ResultConfiguratorBase
                .Type(ProblemsTypesConstantsRfc7807.NotFound.Type)
                .Title(ProblemsTypesConstantsRfc7807.NotFound.Title)
                .Status(ProblemsTypesConstantsRfc7807.NotFound.Status);
        }

        public NotFoundResultBuilderRfc7807 Builder
        {
            get
            {
                _objectResolver.Register(_rfc7807ResultConfiguratorBase
                    .ConfigureResult());

                return _objectResolver.ResolveRequired<NotFoundResultBuilderRfc7807>();
            }
        }

        public NotFoundResultConfiguratorRfc7807 Title(string? title)
        {
            _rfc7807ResultConfiguratorBase.Title(title);
            return this;
        }

        public NotFoundResultConfiguratorRfc7807 Detail(string? detail)
        {
            _rfc7807ResultConfiguratorBase.Detail(detail);
            return this;
        }

        public NotFoundResultConfiguratorRfc7807 Extensions(params
            (string paramName, object? value)[] extensions)
        {
            _rfc7807ResultConfiguratorBase.Extensions(extensions);
            return this;
        }
    }
}