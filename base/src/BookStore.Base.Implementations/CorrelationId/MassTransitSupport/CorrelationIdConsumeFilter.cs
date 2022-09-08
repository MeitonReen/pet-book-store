using BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions;
using MassTransit;

namespace BookStore.Base.Implementations.CorrelationId.MassTransitSupport
{
    public class CorrelationIdConsumeFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
        where TMessage : class
    {
        private readonly ICorrelationIdAccessor? _correlationIdAccessor;

        public CorrelationIdConsumeFilter(
            ICorrelationIdAccessor? correlationIdAccessor
        )
        {
            _correlationIdAccessor = correlationIdAccessor;
        }

        public async Task Send(ConsumeContext<TMessage> context,
            IPipe<ConsumeContext<TMessage>> next)
        {
            //_correlationIdAccessor is scoped
            if (_correlationIdAccessor != default)
            {
                _correlationIdAccessor.CorrelationId ??= context.CorrelationId?.ToString();
            }

            await next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}