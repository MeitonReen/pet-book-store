using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.__Obsolete;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.NotFound.Configurators;
using BookStore.Base.Implementations.Result.Builders.NotFound.Configurators.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.Result.Builders.NotFound.Builders.Rfc7807
{
    public class NotFoundResultBuilderRfc7807 : FluentApiBase
    {
        public NotFoundResultBuilderRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public NotFoundResultConfiguratorRfc7807 Configurator =>
            _objectResolver.ResolveRequired<NotFoundResultConfiguratorRfc7807>();

        public ResultModelGeneric<ProblemDetails> Build()
        {
            var environmentAccessor = _objectResolver.Resolve<EnvironmentAccessor>();

            if (environmentAccessor == default ||
                environmentAccessor.Value == Constants.EnvironmentNames.Production)
            {
                return new ResultModelGeneric<ProblemDetails>
                {
                    ResultStatus = ResultStatus.NotFound,
                    Result = new ResultConfiguratorBaseRfc7807()
                        .Type(ProblemsTypesConstantsRfc7807.NotFound.Type)
                        .Title(ProblemsTypesConstantsRfc7807.NotFound.Title)
                        .Status(ProblemsTypesConstantsRfc7807.NotFound.Status)
                        .ConfigureResult()
                };
            }

            return new ResultModelGeneric<ProblemDetails>
            {
                ResultStatus = ResultStatus.NotFound,
                Result = _objectResolver.ResolveRequired<ProblemDetails>()
            };
        }

        public NotFoundResultBuilderRfc7807 Environment(string environment)
        {
            _objectResolver
                .ResolveRequired<NotFoundResultConfiguratorRoot>()
                .Environment(environment);
            return this;
        }
    }
}