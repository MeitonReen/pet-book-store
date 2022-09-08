using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.FluentApi;

namespace BookStore.Base.Implementations.Result.Builders.Created.Builders
{
    public class CreatedDecoratedResultModelBuilder : FluentApiBase
    {
        public CreatedDecoratedResultModelBuilder(IObjectResolver objectResolver)
            : base(objectResolver)
        {
        }

        public ResultModelGeneric<DecoratedResult> Build()
        {
            var decoratedResult = _objectResolver.ResolveRequired<DecoratedResult>();

            var result = new ResultModelGeneric<DecoratedResult>(decoratedResult,
                ResultStatus.Created);
            return result;
        }
    }
}