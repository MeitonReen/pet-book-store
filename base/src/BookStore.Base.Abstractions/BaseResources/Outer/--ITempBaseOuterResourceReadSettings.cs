namespace BookStore.Base.Abstractions.BaseResources.Outer;

public interface ITempBaseOuterResourceReadSettings<TResourceEntity>
    where TResourceEntity : class
{
    void Messaging<TReadMessage, TResponseMessage>(
        TReadMessage readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        TReadMessage readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        TReadMessage readMessage,
        Func<TOtherResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        TReadMessage readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        TReadMessage readMessage,
        Func<TOtherResponseMessage1, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        TReadMessage readMessage,
        Func<TOtherResponseMessage2, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class;

    void Messaging<TReadMessage, TResponseMessage>(
        object readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        object readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        object readMessage,
        Func<TOtherResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        object readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        object readMessage,
        Func<TOtherResponseMessage1, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class;

    void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        object readMessage,
        Func<TOtherResponseMessage2, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class;
}