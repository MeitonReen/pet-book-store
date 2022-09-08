namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;

public interface IBaseOuterResourceReadSettingsStep1<TResourceEntity>
    where TResourceEntity : class
{
    IBaseOuterResourceReadSettingsStep2<TResourceEntity, TReadMessage>
        ReadMessage<TReadMessage>(TReadMessage message)
        where TReadMessage : class;

    IBaseOuterResourceReadSettingsStep2<TResourceEntity, TReadMessage>
        ReadMessage<TReadMessage>(object message)
        where TReadMessage : class;
}