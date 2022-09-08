using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;
using MassTransit;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit.ReadSettingsSteps;

public class BaseOuterResourceReadSettingsStep2ByMassTransit<TResourceEntity, TReadMessage>
    : IBaseOuterResourceReadSettingsStep2<TResourceEntity, TReadMessage>
    where TResourceEntity : class
    where TReadMessage : class
{
    private readonly Action<Func<Task<TResourceEntity?>>> _queryConsumer;
    private readonly TReadMessage? _readMessage;
    private readonly object? _readMessageObj;
    private readonly IRequestClient<TReadMessage> _requestClient;

    public BaseOuterResourceReadSettingsStep2ByMassTransit(
        IRequestClient<TReadMessage> requestClient,
        Action<Func<Task<TResourceEntity?>>> queryConsumer,
        TReadMessage readMessage,
        object? readMessageObj = default)
    {
        _queryConsumer = queryConsumer;
        _readMessage = readMessage;
        _requestClient = requestClient;
        _readMessageObj = readMessageObj;
    }

    public BaseOuterResourceReadSettingsStep2ByMassTransit(
        IRequestClient<TReadMessage> requestClient,
        Action<Func<Task<TResourceEntity?>>> queryConsumer,
        object readMessageObj,
        TReadMessage? readMessage = default)
    {
        _queryConsumer = queryConsumer;
        _readMessage = readMessage;
        _readMessageObj = readMessageObj;
        _requestClient = requestClient;
    }

    public IBaseOuterResourceReadSettingsStep3V1<
            TResourceEntity, TReadMessage, TResultMessage>
        ResultMessage<TResultMessage>()
        where TResultMessage : class
        => _readMessage != default
            ? new BaseOuterResourceReadSettingsStep3V1ByMassTransit<
                TResourceEntity, TReadMessage, TResultMessage>(
                _requestClient, _queryConsumer, _readMessage)
            : new BaseOuterResourceReadSettingsStep3V1ByMassTransit<
                TResourceEntity, TReadMessage, TResultMessage>(
                _requestClient, _queryConsumer, _readMessageObj!);

    public IBaseOuterResourceReadSettingsStep3V2<
            TResourceEntity, TReadMessage, TResultMessage1, TResultMessage2>
        ResultMessage<TResultMessage1, TResultMessage2>()
        where TResultMessage1 : class
        where TResultMessage2 : class
        => _readMessage != default
            ? new BaseOuterResourceReadSettingsStep3V2ByMassTransit<
                TResourceEntity, TReadMessage, TResultMessage1, TResultMessage2>(
                _requestClient, _queryConsumer, _readMessage)
            : new BaseOuterResourceReadSettingsStep3V2ByMassTransit<
                TResourceEntity, TReadMessage, TResultMessage1, TResultMessage2>(
                _requestClient, _queryConsumer, _readMessageObj!);

    public IBaseOuterResourceReadSettingsStep3V3<TResourceEntity, TReadMessage,
            TResultMessage1, TResultMessage2, TResultMessage3>
        ResultMessage<TResultMessage1, TResultMessage2, TResultMessage3>()
        where TResultMessage1 : class
        where TResultMessage2 : class
        where TResultMessage3 : class
        => _readMessage != default
            ? new BaseOuterResourceReadSettingsStep3V3ByMassTransit<TResourceEntity, TReadMessage,
                TResultMessage1, TResultMessage2, TResultMessage3>(
                _requestClient, _queryConsumer, _readMessage)
            : new BaseOuterResourceReadSettingsStep3V3ByMassTransit<TResourceEntity, TReadMessage,
                TResultMessage1, TResultMessage2, TResultMessage3>(
                _requestClient, _queryConsumer, _readMessageObj!);
}