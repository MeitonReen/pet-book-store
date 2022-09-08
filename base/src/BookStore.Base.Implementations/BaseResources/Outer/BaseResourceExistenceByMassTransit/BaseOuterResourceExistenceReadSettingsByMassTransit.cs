using BookStore.Base.Abstractions.BaseResources.Outer.BaseResourceExistence;
using BookStore.Base.InterserviceContracts.Base.V1_0_0;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceExistenceByMassTransit;

public class BaseOuterResourceExistenceReadSettingsByMassTransit :
    IBaseOuterResourceExistenceReadSettings
{
    private readonly Action<Func<Task<bool>>> _queryConsumer;
    private readonly IServiceProvider _serviceProvider;

    public BaseOuterResourceExistenceReadSettingsByMassTransit(
        IServiceProvider serviceProvider,
        Action<Func<Task<bool>>> queryConsumer
    )
    {
        _serviceProvider = serviceProvider;
        _queryConsumer = queryConsumer;
    }

    public void ReadMessage<TReadMessage>(TReadMessage readMessage)
        where TReadMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = await requestClient.GetResponse<Found, NotFound>(
                readMessage);

            return response.Is(out Response<Found>? _);
        });
    }

    public void ReadMessage<TReadMessage>(object readMessage)
        where TReadMessage : class
    {
        var requestClient = _serviceProvider
            .GetRequiredService<IRequestClient<TReadMessage>>();

        _queryConsumer(async () =>
        {
            var response = await requestClient.GetResponse<Found, NotFound>(
                readMessage);

            return response.Is(out Response<Found>? _);
        });
    }
}