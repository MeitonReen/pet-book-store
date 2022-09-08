using BookStore.Base.Abstractions.BaseResources.Outer.BaseResource.ReadSettingsSteps;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;

namespace BookStore.Base.Implementations.BaseResources.Outer.BaseResourceByMassTransit.ReadSettingsSteps;

public class BaseOuterResourceReadSettingsStep1ByMassTransit<TResourceEntity>
    : IBaseOuterResourceReadSettingsStep1<TResourceEntity>
    where TResourceEntity : class
{
    private readonly Action<Func<Task<TResourceEntity?>>> _queryConsumer;
    private readonly IServiceProvider _serviceProvider;

    public BaseOuterResourceReadSettingsStep1ByMassTransit(
        IServiceProvider serviceProvider,
        Action<Func<Task<TResourceEntity?>>> queryConsumer
    )
    {
        _serviceProvider = serviceProvider;
        _queryConsumer = queryConsumer;
    }

    public IBaseOuterResourceReadSettingsStep2<TResourceEntity, TReadMessage>
        ReadMessage<TReadMessage>(TReadMessage message)
        where TReadMessage : class
        => new BaseOuterResourceReadSettingsStep2ByMassTransit<TResourceEntity, TReadMessage>(
            _serviceProvider.GetRequiredService<IRequestClient<TReadMessage>>(),
            _queryConsumer, message);

    public IBaseOuterResourceReadSettingsStep2<TResourceEntity, TReadMessage>
        ReadMessage<TReadMessage>(object message)
        where TReadMessage : class
        => new BaseOuterResourceReadSettingsStep2ByMassTransit<TResourceEntity, TReadMessage>(
            _serviceProvider.GetRequiredService<IRequestClient<TReadMessage>>(),
            _queryConsumer, message);
}