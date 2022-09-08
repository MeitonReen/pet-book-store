using BookStore.Base.Abstractions.BaseResources.Outer;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit;

public class TempTempBaseOuterResourceReadSettingsByMassTransit<TResourceEntity>
    : ITempBaseOuterResourceReadSettings<TResourceEntity>
    where TResourceEntity : class
{
    private readonly Action<Func<Task<TResourceEntity?>>> _queryConsumer;
    private readonly IServiceProvider _serviceProvider;

    public TempTempBaseOuterResourceReadSettingsByMassTransit(
        IServiceProvider serviceProvider,
        Action<Func<Task<TResourceEntity?>>> queryConsumer
    )
    {
        _serviceProvider = serviceProvider;
        _queryConsumer = queryConsumer;
    }

    public void Messaging<TReadMessage, TResponseMessage>(
        TReadMessage readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = await requestClient.GetResponse<TResponseMessage>(readMessage);

            return responseMessageToResourceEntityConverter(response.Message);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        TReadMessage readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage>(readMessage));

            var responseMessage = response.Is(
                out Response<TResponseMessage>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        TReadMessage readMessage,
        Func<TOtherResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage>(readMessage));

            var responseMessage = response.Is(
                out Response<TOtherResponseMessage>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        TReadMessage readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage1,
                    TOtherResponseMessage2>(readMessage));

            var responseMessage = response.Is(
                out Response<TResponseMessage>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        TReadMessage readMessage,
        Func<TOtherResponseMessage1, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage1,
                    TOtherResponseMessage2>(readMessage));

            var responseMessage = response.Is(
                out Response<TOtherResponseMessage1>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        TReadMessage readMessage,
        Func<TOtherResponseMessage2, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage1,
                    TOtherResponseMessage2>(readMessage));

            var responseMessage = response.Is(
                out Response<TOtherResponseMessage2>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage>(
        object readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = await requestClient.GetResponse<TResponseMessage>(readMessage);

            return responseMessageToResourceEntityConverter(response.Message);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        object readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage>(readMessage));

            var responseMessage = response.Is(
                out Response<TResponseMessage>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage>(
        object readMessage,
        Func<TOtherResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage>(readMessage));

            var responseMessage = response.Is(
                out Response<TOtherResponseMessage>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        object readMessage,
        Func<TResponseMessage, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage1,
                    TOtherResponseMessage2>(readMessage));

            var responseMessage = response.Is(
                out Response<TResponseMessage>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        object readMessage,
        Func<TOtherResponseMessage1, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage1,
                    TOtherResponseMessage2>(readMessage));

            var responseMessage = response.Is(
                out Response<TOtherResponseMessage1>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }

    public void Messaging<TReadMessage, TResponseMessage, TOtherResponseMessage1,
        TOtherResponseMessage2>(
        object readMessage,
        Func<TOtherResponseMessage2, TResourceEntity> responseMessageToResourceEntityConverter)
        where TReadMessage : class
        where TResponseMessage : class
        where TOtherResponseMessage1 : class
        where TOtherResponseMessage2 : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = (await requestClient
                .GetResponse<TResponseMessage, TOtherResponseMessage1,
                    TOtherResponseMessage2>(readMessage));

            var responseMessage = response.Is(
                out Response<TOtherResponseMessage2>? resourceMessage)
                ? resourceMessage?.Message
                : default;

            return responseMessage == default
                ? default
                : responseMessageToResourceEntityConverter(responseMessage);
        });
    }
}