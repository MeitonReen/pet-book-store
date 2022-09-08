using BookStore.Base.Implementations.LoggingInterpolationSupport;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.BookStoreDefaultLogging.Masstransit;

public class LoggingFilters<TMessage> :
    IFilter<ConsumeContext<TMessage>>,
    IFilter<PublishContext<TMessage>>,
    IFilter<SendContext<TMessage>>
    where TMessage : class
{
    private readonly ILogger<LoggingFilters<TMessage>>? _logger;

    public LoggingFilters(
        ILogger<LoggingFilters<TMessage>>? logger)
    {
        _logger = logger;
    }

    public async Task Send(ConsumeContext<TMessage> context,
        IPipe<ConsumeContext<TMessage>> next)
    {
        if (_logger == default)
        {
            await next.Send(context);
            return;
        }

        _logger.LogInformation(
            $"\n{typeof(TMessage).Name} message processing was started by consumer;"
            + $"\nmessage: @{context.Message};"
            + $"\ncorrelationId: {context.CorrelationId}.");

        try
        {
            await next.Send(context);

            _logger.LogInformation(
                $"\n{typeof(TMessage).Name} message processing successfully completed;"
                + $"\ncorrelationId: {context.CorrelationId}.");
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"\nException was thrown when processing {typeof(TMessage).Name} message by consumer: {e.Message};"
                + $"\ninner exception: {e.InnerException?.Message};"
                + $"\nmessage: @{context.Message};"
                + $"\ncorrelationId: {context.CorrelationId}.");

            throw;
        }
    }

    public void Probe(ProbeContext context)
    {
    }

    public async Task Send(PublishContext<TMessage> context, IPipe<PublishContext<TMessage>> next)
    {
        if (_logger == default)
        {
            await next.Send(context);
            return;
        }

        _logger.LogInformation(
            $"\nPublishing {typeof(TMessage).Name} message was started;"
            + $"\nmessage: @{context.Message};"
            + $"\ncorrelationId: {context.CorrelationId}.");

        try
        {
            await next.Send(context);

            _logger.LogInformation(
                $"\nPublishing {typeof(TMessage).Name} message successfully completed;"
                + $"\ncorrelationId: {context.CorrelationId}.");
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"\nWas thrown exception when publishing {typeof(TMessage).Name} message: {e.Message};"
                + $"\ninner exception: {e.InnerException?.Message};"
                + $"\nmessage: @{context.Message};"
                + $"\ncorrelationId: {context.CorrelationId}.");

            throw;
        }
    }

    public async Task Send(SendContext<TMessage> context, IPipe<SendContext<TMessage>> next)
    {
        if (_logger == default)
        {
            await next.Send(context);
            return;
        }

        _logger.LogInformation(
            $"\nSending {typeof(TMessage).Name} message was started;"
            + $"\nmessage: @{context.Message};"
            + $"\ncorrelationId: {context.CorrelationId}.");

        try
        {
            await next.Send(context);

            _logger.LogInformation(
                $"\nSending {typeof(TMessage).Name} message successfully completed;"
                + $"\ncorrelationId: {context.CorrelationId}.");
        }
        catch (Exception e)
        {
            _logger.LogError(
                $"\nWas thrown exception when sending {typeof(TMessage).Name} message: {e.Message};"
                + $"\ninner exception: {e.InnerException?.Message};"
                + $"\nmessage: @{context.Message};"
                + $"\ncorrelationId: {context.CorrelationId}.");

            throw;
        }
    }
}