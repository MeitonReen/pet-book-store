using BookStore.Base.Abstractions.ObjectResolver;

namespace BookStore.Base.Implementations.ObjectResolver
{
    public class ObjectResolver : IObjectResolver
    {
        private readonly Dictionary<Type, object> _objectDictionary = new();

        public TTarget ResolveRequired<TTarget>() where TTarget : class
        {
            if (!_objectDictionary.TryGetValue(typeof(TTarget), out var resolveValue))
            {
                throw new NullReferenceException($"{nameof(ObjectResolver)} can't resolve {typeof(TTarget).Name} type");
            }

            return (TTarget) resolveValue;
        }

        public TTarget? Resolve<TTarget>() where TTarget : class
        {
            if (!_objectDictionary.TryGetValue(typeof(TTarget), out var resolveValue))
            {
                return default;
            }

            return (TTarget) resolveValue;
        }

        public void Register<TTarget>(TTarget @object) where TTarget : class
        {
            Register(typeof(TTarget), @object);
        }

        public void Register(Type type, object @object)
        {
            if (@object != default)
            {
                _objectDictionary.TryAdd(type, @object);
            }
        }

        public bool TryRegister<TTarget>(TTarget @object) where TTarget : class
        {
            return _objectDictionary.TryAdd(typeof(TTarget), @object);
        }

        public void Replace<TTarget>(TTarget @object) where TTarget : class
        {
            if (_objectDictionary.ContainsKey(typeof(TTarget)))
            {
                _objectDictionary[typeof(TTarget)] = @object;
            }
            else
            {
                Register(@object);
            }
        }
    }
}