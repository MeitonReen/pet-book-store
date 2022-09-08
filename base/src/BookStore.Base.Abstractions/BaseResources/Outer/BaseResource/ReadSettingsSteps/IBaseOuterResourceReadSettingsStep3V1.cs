namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;

public interface IBaseOuterResourceReadSettingsStep3V1<TResourceEntity, TReadMessage,
    TResultMessage>
    where TResourceEntity : class
    where TReadMessage : class
    where TResultMessage : class
{
    void ConverterToResourceEntity(Func<TResultMessage, TResourceEntity> convert);
}