using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.__Obsolete;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Configurators;
using BookStore.Base.Implementations.Result.Builders.BadRequest.Configurators.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.Result.Builders.BadRequest.Builders.Rfc7807
{
    public class BadRequestResultBuilderRfc7807 : FluentApiBase
    {
        public BadRequestResultBuilderRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public BadRequestResultConfiguratorRfc7807 Configurator =>
            _objectResolver.ResolveRequired<BadRequestResultConfiguratorRfc7807>();

        public ResultModelGeneric<ProblemDetails> Build()
        {
            var environmentAccessor = _objectResolver.Resolve<EnvironmentAccessor>();

            if (environmentAccessor == default ||
                environmentAccessor.Value == Constants.EnvironmentNames.Production)
            {
                return new ResultModelGeneric<ProblemDetails>
                {
                    ResultStatus = ResultStatus.BadRequest,
                    Result = new ResultConfiguratorBaseRfc7807()
                        .Type(ProblemsTypesConstantsRfc7807.BadRequest.Type)
                        .Title(ProblemsTypesConstantsRfc7807.BadRequest.Title)
                        .Status(ProblemsTypesConstantsRfc7807.BadRequest.Status)
                        .ConfigureResult()
                };
            }

            return new ResultModelGeneric<ProblemDetails>
            {
                ResultStatus = ResultStatus.BadRequest,
                Result = _objectResolver.ResolveRequired<ProblemDetails>()
            };
        }

        public BadRequestResultBuilderRfc7807 Environment(string environment)
        {
            _objectResolver
                .ResolveRequired<BadRequestResultConfiguratorRoot>()
                .Environment(environment);
            return this;
        }
    }
}