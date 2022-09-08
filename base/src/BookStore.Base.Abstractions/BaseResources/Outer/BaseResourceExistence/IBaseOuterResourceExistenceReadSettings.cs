namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence;

public interface IBaseOuterResourceExistenceReadSettings
{
    void ReadMessage<TReadMessage>(TReadMessage readMessage)
        where TReadMessage : class;

    void ReadMessage<TReadMessage>(object readMessage)
        where TReadMessage : class;
}