using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Exception.Builders.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;

namespace BookStore.Base.Implementations.Result.Builders.Exception.Configurators.Rfc7807
{
    public class ExceptionResultConfiguratorRfc7807 : FluentApiBase
    {
        private readonly ResultConfiguratorBaseRfc7807 _rfc7807ResultConfiguratorBase = new();

        public ExceptionResultConfiguratorRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
            _rfc7807ResultConfiguratorBase
                .Type(ProblemsTypesConstantsRfc7807.Exception.Type)
                .Title(ProblemsTypesConstantsRfc7807.Exception.Title)
                .Status(ProblemsTypesConstantsRfc7807.Exception.Status);
        }

        public ExceptionResultBuilderRfc7807 Builder
        {
            get
            {
                _objectResolver.Register(_rfc7807ResultConfiguratorBase
                    .ConfigureResult());

                return _objectResolver.ResolveRequired<ExceptionResultBuilderRfc7807>();
            }
        }

        public ExceptionResultConfiguratorRfc7807 Title(string? title)
        {
            _rfc7807ResultConfiguratorBase.Title(title);
            return this;
        }

        public ExceptionResultConfiguratorRfc7807 Detail(string? detail)
        {
            _rfc7807ResultConfiguratorBase.Detail(detail);
            return this;
        }

        public ExceptionResultConfiguratorRfc7807 Extensions(params
            (string paramName, object? value)[] extensions)
        {
            _rfc7807ResultConfiguratorBase.Extensions(extensions);
            return this;
        }
    }
}