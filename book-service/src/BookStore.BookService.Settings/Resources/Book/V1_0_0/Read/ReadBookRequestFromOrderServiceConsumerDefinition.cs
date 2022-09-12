using BookStore.BookService.Data.BaseDatabase;
using BookStore.BookService.WebEntryPoint.Book.V1_0_0.Read;
using MassTransit;

namespace BookStore.BookService.Settings.Resources.Book.V1_0_0.Read;

public class ReadBookRequestFromOrderServiceConsumerDefinition :
    ConsumerDefinition<ReadBookRequestFromOrderServiceConsumer>
{
    readonly IServiceProvider _provider;

    public ReadBookRequestFromOrderServiceConsumerDefinition(IServiceProvider provider)
    {
        _provider = provider;
    }

    protected override void ConfigureConsumer(IReceiveEndpointConfigurator endpointConfigurator,
        IConsumerConfigurator<ReadBookRequestFromOrderServiceConsumer> consumerConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<BaseDbContext>(_provider);
    }
}