using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.__Obsolete;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Exception.Configurators;
using BookStore.Base.Implementations.Result.Builders.Exception.Configurators.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.Result.Builders.Exception.Builders.Rfc7807
{
    public class ExceptionResultBuilderRfc7807 : FluentApiBase
    {
        public ExceptionResultBuilderRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ExceptionResultConfiguratorRfc7807 Configurator =>
            _objectResolver.ResolveRequired<ExceptionResultConfiguratorRfc7807>();

        public ResultModelGeneric<ProblemDetails> Build()
        {
            var environmentAccessor = _objectResolver.Resolve<EnvironmentAccessor>();

            if (environmentAccessor == default ||
                environmentAccessor.Value == Constants.EnvironmentNames.Production)
            {
                return new ResultModelGeneric<ProblemDetails>
                {
                    ResultStatus = ResultStatus.Exception,
                    Result = new ResultConfiguratorBaseRfc7807()
                        .Type(ProblemsTypesConstantsRfc7807.Exception.Type)
                        .Title(ProblemsTypesConstantsRfc7807.Exception.Title)
                        .Status(ProblemsTypesConstantsRfc7807.Exception.Status)
                        .Detail("The service is temporarily unavailable, try it in a minute")
                        .ConfigureResult()
                };
            }

            return new ResultModelGeneric<ProblemDetails>
            {
                ResultStatus = ResultStatus.Exception,
                Result = _objectResolver.ResolveRequired<ProblemDetails>()
            };
        }

        public ExceptionResultBuilderRfc7807 Environment(string environment)
        {
            _objectResolver
                .ResolveRequired<ExceptionResultConfiguratorRoot>()
                .Environment(environment);
            return this;
        }
    }
}