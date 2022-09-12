using BookStore.BookService.Data.Book.V1_0_0.UpdateOut.SagaInstance;
using BookStore.BookService.Data.SagasDatabase;
using MassTransit;

namespace BookStore.BookService.Settings.Resources.Book.V1_0_0.UpdateOut;

public class SagaOrchestratorDefinition : SagaDefinition<SagaOrchestratorInstance>
{
    readonly IServiceProvider _serviceProvider;

    public SagaOrchestratorDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<SagaOrchestratorInstance> consumerConfigurator)
    {
        endpointConfigurator.UseEntityFrameworkOutbox<SagasDbContext>(_serviceProvider);
    }
}