using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Conflict.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.Conflict.Configurators.Rfc7807
{
    public class ConflictResultConfiguratorRfc7807 : FluentApiBase
    {
        private readonly ResultConfiguratorBaseRfc7807 _rfc7807ResultConfiguratorBase = new();

        public ConflictResultConfiguratorRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
            _rfc7807ResultConfiguratorBase
                .Type(ProblemsTypesConstantsRfc7807.Conflict.Type)
                .Title(ProblemsTypesConstantsRfc7807.Conflict.Title)
                .Status(ProblemsTypesConstantsRfc7807.Conflict.Status);
        }

        public ConflictResultBuilderRfc7807 Builder
        {
            get
            {
                _objectResolver.Register(_rfc7807ResultConfiguratorBase
                    .ConfigureResult());

                return _objectResolver.ResolveRequired<ConflictResultBuilderRfc7807>();
            }
        }

        public ConflictResultConfiguratorRfc7807 Title(string? title)
        {
            _rfc7807ResultConfiguratorBase.Title(title);
            return this;
        }

        public ConflictResultConfiguratorRfc7807 Detail(string? detail)
        {
            _rfc7807ResultConfiguratorBase.Detail(detail);
            return this;
        }

        public ConflictResultConfiguratorRfc7807 Extensions(params
            (string paramName, object? value)[] extensions)
        {
            _rfc7807ResultConfiguratorBase.Extensions(extensions);
            return this;
        }
    }
}