namespace BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions
{
    public interface ICorrelationIdAccessor
    {
        string? CorrelationId { get; set; }
    }
}