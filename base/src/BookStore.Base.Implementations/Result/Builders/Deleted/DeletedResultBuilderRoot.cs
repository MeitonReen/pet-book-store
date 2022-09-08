using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.FluentApi;

namespace BookStore.Base.Implementations.Result.Builders.Deleted
{
    public class DeletedResultBuilderRoot<TResult> : FluentApiBase
    {
        private readonly TResult _result;

        public DeletedResultBuilderRoot(IObjectResolver objectResolver, TResult result)
            : base(objectResolver)
        {
            _result = result;
        }

        public ResultModelGeneric<TResult> Build() => new(_result, ResultStatus.Deleted);
    }
}