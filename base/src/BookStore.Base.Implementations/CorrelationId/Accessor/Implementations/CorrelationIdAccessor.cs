using BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions;

namespace BookStore.Base.Implementations.CorrelationId.Accessor.Implementations
{
    public class CorrelationIdAccessor : ICorrelationIdAccessor
    {
        public string CorrelationId { get; set; } = string.Empty;
    }
}