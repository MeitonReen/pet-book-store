using BookStore.UserService.Data.BaseDatabase;
using BookStore.UserService.WebEntryPoint.ProfileExistence.V1_0_0.Read;
using MassTransit;

namespace BookStore.UserService.Settings.Resources.ProfileExistence.V1_0_0.Read;

public class ReadProfileExistenceRequestConsumerDefinition :
    ConsumerDefinition<ReadProfileExistenceRequestConsumer>
{
    readonly IServiceProvider _provider;

    public ReadProfileExistenceRequestConsumerDefinition(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ReadProfileExistenceRequestConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_provider);
    }
}