using MassTransit;

namespace BookStore.BookService.Data.Book.V1_0_0.UpdateOut.SagaInstance;

public class SagaOrchestratorInstance : SagaStateMachineInstance
{
    public int CurrentState { get; set; }
    public Uri ResponseAddress { get; set; }
    public Guid RequestId { get; set; }
    public Uri OrchestratorInstanceAddress { get; set; }

    public uint ConcurrencyToken { get; set; }
    public Guid CorrelationId { get; set; }
}