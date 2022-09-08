using BookStore.Base.Abstractions.ObjectResolver;

namespace BookStore.Base.Implementations.FluentApi
{
    public class FluentApiBase
    {
        protected IObjectResolver _objectResolver;

        public FluentApiBase(IObjectResolver? objectResolver)
        {
            _objectResolver = objectResolver ??
                              throw new ArgumentNullException(nameof(ObjectResolver),
                                  $"{nameof(FluentApiBase)}.ctor --> {nameof(ObjectResolver)} is required"
                              );

            _objectResolver.Register(GetType(), this);
        }

        public static ObjectResolver.ObjectResolver ObjectResolverFirst
        {
            get => new();
        }
    }
}