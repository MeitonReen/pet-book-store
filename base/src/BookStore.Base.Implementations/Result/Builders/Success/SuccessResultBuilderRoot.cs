using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.FluentApi;

namespace BookStore.Base.Implementations.Result.Builders.Success
{
    public class SuccessResultBuilderRoot<TResult> : FluentApiBase
    {
        private readonly TResult _result;

        public SuccessResultBuilderRoot(IObjectResolver objectResolver, TResult result)
            : base(objectResolver)
        {
            _result = result;
        }

        public ResultModelGeneric<TResult> Build() => new(_result, ResultStatus.Success);
    }
}