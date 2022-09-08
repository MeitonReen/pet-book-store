using MassTransit;

namespace BookStore.UserService.Data.Profile.V1_0_0.DeleteOut.SagaInstance;

public class SagaOrchestratorInstance : SagaStateMachineInstance
{
    public int CurrentState { get; set; }
    public Uri ResponseAddress { get; set; }
    public Guid RequestId { get; set; }
    public Uri OrchestratorInstanceAddress { get; set; }
    public uint ConcurrencyToken { get; set; }
    public Guid CorrelationId { get; set; }
}