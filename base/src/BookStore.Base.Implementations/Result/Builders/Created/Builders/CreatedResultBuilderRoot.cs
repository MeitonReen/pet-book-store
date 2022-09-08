using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Contracts.Abstractions.Result;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Created.Decorators;

namespace BookStore.Base.Implementations.Result.Builders.Created.Builders
{
    public class CreatedResultBuilderRoot<TResult> : FluentApiBase where TResult : class
    {
        public CreatedResultBuilderRoot(IObjectResolver objectResolver, TResult result)
            : base(objectResolver) => _objectResolver.Register(result);

        public CreatedResultDecoratorRoot<TResult> Decorator
        {
            get => new(_objectResolver);
        }

        public ResultModelGeneric<TResult> Build() =>
            new(_objectResolver.ResolveRequired<TResult>(), ResultStatus.Created);
    }
}