using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.BadRequest.Configurators.Rfc7807
{
    public class BadRequestResultConfiguratorRfc7807 : FluentApiBase
    {
        private readonly ResultConfiguratorBaseRfc7807 _rfc7807ResultConfiguratorBase = new();

        public BadRequestResultConfiguratorRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
            _rfc7807ResultConfiguratorBase
                .Type(ProblemsTypesConstantsRfc7807.BadRequest.Type)
                .Title(ProblemsTypesConstantsRfc7807.BadRequest.Title)
                .Status(ProblemsTypesConstantsRfc7807.BadRequest.Status);
        }

        public BadRequestResultBuilderRfc7807 Builder
        {
            get
            {
                _objectResolver.Register(_rfc7807ResultConfiguratorBase
                    .ConfigureResult());

                return _objectResolver.ResolveRequired<BadRequestResultBuilderRfc7807>();
            }
        }

        public BadRequestResultConfiguratorRfc7807 Title(string? title)
        {
            _rfc7807ResultConfiguratorBase.Title(title);
            return this;
        }

        public BadRequestResultConfiguratorRfc7807 Detail(string? detail)
        {
            _rfc7807ResultConfiguratorBase.Detail(detail);
            return this;
        }

        public BadRequestResultConfiguratorRfc7807 Extensions(params
            (string paramName, object? value)[] extensions)
        {
            _rfc7807ResultConfiguratorBase.Extensions(extensions);
            return this;
        }
    }
}