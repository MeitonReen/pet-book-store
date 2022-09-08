namespace BookStore.Base.Abstractions.ObjectResolver
{
    public interface IObjectResolver
    {
        TTarget ResolveRequired<TTarget>() where TTarget : class;
        TTarget? Resolve<TTarget>() where TTarget : class;
        void Register<TTarget>(TTarget @object) where TTarget : class;
        bool TryRegister<TTarget>(TTarget @object) where TTarget : class;
        void Register(Type type, object @object);
        void Replace<TTarget>(TTarget @object) where TTarget : class;
    }
}