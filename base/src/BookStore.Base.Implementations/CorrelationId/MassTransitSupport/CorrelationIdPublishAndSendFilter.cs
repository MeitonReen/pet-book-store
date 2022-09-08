using BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions;
using MassTransit;

namespace BookStore.Base.Implementations.CorrelationId.MassTransitSupport
{
    public class CorrelationIdPublishAndSendFilter<TMessage> :
        IFilter<PublishContext<TMessage>>,
        IFilter<SendContext<TMessage>>
        where TMessage : class
    {
        private readonly ICorrelationIdAccessor? _correlationIdAccessor;

        public CorrelationIdPublishAndSendFilter(
            ICorrelationIdAccessor? correlationIdAccessor)
        {
            _correlationIdAccessor = correlationIdAccessor;
        }

        public Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
        {
            if (Guid.TryParse(_correlationIdAccessor?.CorrelationId, out var correlationId))
            {
                context.CorrelationId = correlationId;
            }

            return next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
        {
            if (Guid.TryParse(_correlationIdAccessor?.CorrelationId, out var correlationId))
            {
                context.CorrelationId = correlationId;
            }

            return next.Send(context);
        }
    }
}