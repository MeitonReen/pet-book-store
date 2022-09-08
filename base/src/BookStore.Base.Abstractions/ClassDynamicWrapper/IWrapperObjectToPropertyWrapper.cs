namespace BookStore.Base.Abstractions.ClassDynamicWrapper;

public interface IWrapperObjectToPropertyWrapper
{
    object Wrap<TToWrap>(TToWrap objectToPropertyWrapper) where TToWrap : class;
}