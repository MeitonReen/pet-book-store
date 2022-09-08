namespace BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;

public interface IBaseOuterResourceReadSettingsStep2<
    TResourceEntity, TReadMessage>
    where TResourceEntity : class
    where TReadMessage : class
{
    IBaseOuterResourceReadSettingsStep3V1<
            TResourceEntity, TReadMessage, TResultMessage>
        ResultMessage<TResultMessage>()
        where TResultMessage : class;

    IBaseOuterResourceReadSettingsStep3V2<TResourceEntity, TReadMessage,
            TResultMessage1, TResultMessage2>
        ResultMessage<TResultMessage1, TResultMessage2>()
        where TResultMessage1 : class
        where TResultMessage2 : class;

    IBaseOuterResourceReadSettingsStep3V3<TResourceEntity, TReadMessage,
            TResultMessage1, TResultMessage2, TResultMessage3>
        ResultMessage<TResultMessage1, TResultMessage2, TResultMessage3>()
        where TResultMessage1 : class
        where TResultMessage2 : class
        where TResultMessage3 : class;
}