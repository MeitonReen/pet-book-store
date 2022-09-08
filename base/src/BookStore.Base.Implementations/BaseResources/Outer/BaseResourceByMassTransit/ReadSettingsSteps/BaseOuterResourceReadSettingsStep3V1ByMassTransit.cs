using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;
using MassTransit;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit.ReadSettingsSteps;

public class BaseOuterResourceReadSettingsStep3V1ByMassTransit<TResourceEntity, TReadMessage,
        TResultMessage>
    : IBaseOuterResourceReadSettingsStep3V1<TResourceEntity, TReadMessage, TResultMessage>
    where TResourceEntity : class
    where TReadMessage : class
    where TResultMessage : class
{
    private readonly Action<Func<Task<TResourceEntity?>>> _queryConsumer;
    private readonly TReadMessage? _readMessage;
    private readonly object? _readMessageObj;
    private readonly IRequestClient<TReadMessage> _requestClient;

    public BaseOuterResourceReadSettingsStep3V1ByMassTransit(
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

    public BaseOuterResourceReadSettingsStep3V1ByMassTransit(
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

    public void ConverterToResourceEntity(Func<TResultMessage, TResourceEntity> convert)
        => _queryConsumer(async () =>
        {
            var response = _readMessage != default
                ? await _requestClient.GetResponse<TResultMessage>(_readMessage)
                : await _requestClient.GetResponse<TResultMessage>(_readMessageObj!);

            return convert(response.Message);
        });
}