using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.WebEntryPoint.BookExistence.V1_0_0.Read;
using MassTransit;

namespace BookStore.BookService.Settings.Resources.BookExistence.V1_0_0.Read;

public class ReadBookExistenceRequestConsumerDefinition :
    ConsumerDefinition<ReadBookExistenceRequestConsumer>
{
    readonly IServiceProvider _provider;

    public ReadBookExistenceRequestConsumerDefinition(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ReadBookExistenceRequestConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_provider);
    }
}