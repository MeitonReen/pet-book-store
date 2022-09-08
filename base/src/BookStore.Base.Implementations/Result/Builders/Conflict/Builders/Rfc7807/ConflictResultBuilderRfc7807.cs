using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.__Obsolete;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Conflict.Configurators;
using BookStore.Base.Implementations.Result.Builders.Conflict.Configurators.Rfc7807;
using BookStore.Base.Implementations.Result.Rfc7807;
using Microsoft.AspNetCore.Mvc;

namespace BookStore.Base.Implementations.Result.Builders.Conflict.Builders.Rfc7807
{
    public class ConflictResultBuilderRfc7807 : FluentApiBase
    {
        public ConflictResultBuilderRfc7807(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ConflictResultConfiguratorRfc7807 Configurator =>
            _objectResolver.ResolveRequired<ConflictResultConfiguratorRfc7807>();

        public ResultModelGeneric<ProblemDetails> Build()
        {
            var environmentAccessor = _objectResolver.Resolve<EnvironmentAccessor>();

            if (environmentAccessor == default ||
                environmentAccessor.Value == Constants.EnvironmentNames.Production)
            {
                return new ResultModelGeneric<ProblemDetails>
                {
                    ResultStatus = ResultStatus.Conflict,
                    Result = new ResultConfiguratorBaseRfc7807()
                        .Type(ProblemsTypesConstantsRfc7807.Conflict.Type)
                        .Title(ProblemsTypesConstantsRfc7807.Conflict.Title)
                        .Status(ProblemsTypesConstantsRfc7807.Conflict.Status)
                        .ConfigureResult()
                };
            }

            return new ResultModelGeneric<ProblemDetails>
            {
                ResultStatus = ResultStatus.Conflict,
                Result = _objectResolver.ResolveRequired<ProblemDetails>()
            };
        }

        public ConflictResultBuilderRfc7807 Environment(string environment)
        {
            _objectResolver
                .ResolveRequired<ConflictResultConfiguratorRoot>()
                .Environment(environment);
            return this;
        }
    }
}