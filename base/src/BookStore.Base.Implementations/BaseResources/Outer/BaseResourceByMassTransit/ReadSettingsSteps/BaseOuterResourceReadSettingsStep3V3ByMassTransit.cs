﻿using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;
using MassTransit;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit.ReadSettingsSteps;

public class BaseOuterResourceReadSettingsStep3V3ByMassTransit<
        TResourceEntity, TReadMessage, TResultMessage1, TResultMessage2, TResultMessage3>
    : IBaseOuterResourceReadSettingsStep3V3<TResourceEntity, TReadMessage,
        TResultMessage1, TResultMessage2, TResultMessage3>
    where TResourceEntity : class
    where TReadMessage : class
    where TResultMessage1 : class
    where TResultMessage2 : class
    where TResultMessage3 : class
{
    private readonly Action<Func<Task<TResourceEntity?>>> _queryConsumer;
    private readonly TReadMessage? _readMessage;
    private readonly object? _readMessageObj;
    private readonly IRequestClient<TReadMessage> _requestClient;

    public BaseOuterResourceReadSettingsStep3V3ByMassTransit(
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

    public BaseOuterResourceReadSettingsStep3V3ByMassTransit(
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

    public void ConverterToResourceEntity(Func<TResultMessage1, TResourceEntity> convert)
        => _queryConsumer(async () =>
        {
            var response = _readMessage != default
                ? await _requestClient
                    .GetResponse<TResultMessage1, TResultMessage2, TResultMessage3>(
                        _readMessage)
                : await _requestClient
                    .GetResponse<TResultMessage1, TResultMessage2, TResultMessage3>(
                        _readMessageObj!);

            var responseMessage = response.Is(
                out Response<TResultMessage1>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : convert(responseMessage);
        });

    public void ConverterToResourceEntity(Func<TResultMessage2, TResourceEntity> convert)
        => _queryConsumer(async () =>
        {
            var response = _readMessage != default
                ? await _requestClient
                    .GetResponse<TResultMessage1, TResultMessage2, TResultMessage3>(
                        _readMessage)
                : await _requestClient
                    .GetResponse<TResultMessage1, TResultMessage2, TResultMessage3>(
                        _readMessageObj!);

            var responseMessage = response.Is(
                out Response<TResultMessage2>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : convert(responseMessage);
        });

    public void ConverterToResourceEntity(Func<TResultMessage3, TResourceEntity> convert)
        => _queryConsumer(async () =>
        {
            var response = _readMessage != default
                ? await _requestClient
                    .GetResponse<TResultMessage1, TResultMessage2, TResultMessage3>(
                        _readMessage)
                : await _requestClient
                    .GetResponse<TResultMessage1, TResultMessage2, TResultMessage3>(
                        _readMessageObj!);

            var responseMessage = response.Is(
                out Response<TResultMessage3>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : convert(responseMessage);
        });
}