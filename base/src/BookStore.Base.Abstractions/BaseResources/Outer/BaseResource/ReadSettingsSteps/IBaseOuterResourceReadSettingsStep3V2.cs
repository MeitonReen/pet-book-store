namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;

public interface IBaseOuterResourceReadSettingsStep3V2<TResourceEntity, TReadMessage,
    TResultMessage1, TResultMessage2>
    where TResourceEntity : class
    where TReadMessage : class
    where TResultMessage1 : class
    where TResultMessage2 : class
{
    void ConverterToResourceEntity(Func<TResultMessage1, TResourceEntity> convert);
    void ConverterToResourceEntity(Func<TResultMessage2, TResourceEntity> convert);
}