using BookStore.BookService.Data.SagasDatabase;
using MassTransit;

namespace BookStore.BookService.Data.Book.V1_0_0.DeleteOut.SagaInstance;

public class SagaOrchestratorInstanceDefinition :
    SagaDefinition<SagaOrchestratorInstance>
{
    readonly IServiceProvider _serviceProvider;

    public SagaOrchestratorInstanceDefinition(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<SagaOrchestratorInstance> consumerConfigurator)
    {
        endpointConfigurator.UseMessageRetry(sets => sets.Intervals(100, 500, 1000, 1000, 1000, 1000, 1000));

        endpointConfigurator.UseEntityFrameworkOutbox<SagasDbContext>(_serviceProvider);
    }
}