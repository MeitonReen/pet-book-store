using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.Result.Builders.Created.Builders;
using BookStore.Base.Implementations.Result.Location;

namespace BookStore.Base.Implementations.Result.Builders.Created.Decorators
{
    public class CreatedResultDecoratorRoot<TResult> : FluentApiBase where TResult : class
    {
        public CreatedResultDecoratorRoot(IObjectResolver objectResolver)
            : base(objectResolver)
        {
            var result = _objectResolver.ResolveRequired<TResult>();
            var decoratedResult = new DecoratedResult {Value = result};

            _objectResolver.Register(decoratedResult);
        }

        public CreatedDecoratedResultModelBuilder Builder
        {
            get => new(_objectResolver);
        }

        public CreatedResultDecoratorRoot<TResult> DecorateLocation<TRequestForLocationParams>(
            TRequestForLocationParams requestForLocationParams)
        {
            var decoratedResult = _objectResolver.ResolveRequired<DecoratedResult>();

            decoratedResult.Value = new LocationResult
            {
                Result = decoratedResult.Value,
                RequestForLocation = requestForLocationParams
            };

            return this;
        }
    }
}