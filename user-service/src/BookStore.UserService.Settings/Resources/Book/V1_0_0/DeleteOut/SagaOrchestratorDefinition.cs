using BookStore.UserService.Data.Profile.V1_0_0.DeleteOut.SagaInstance;
using BookStore.UserService.Data.SagasDatabase;
using MassTransit;

namespace BookStore.UserService.Settings.Resources.Book.V1_0_0.DeleteOut;

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