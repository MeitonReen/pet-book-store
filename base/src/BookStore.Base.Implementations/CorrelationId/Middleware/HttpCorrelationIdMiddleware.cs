using BookStore.Base.Implementations.CorrelationId.Accessor.Abstractions;
using BookStore.Base.Implementations.CorrelationId.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace BookStore.Base.Implementations.CorrelationId.Middleware
{
    public class HttpCorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly HttpCorrelationIdOptions _options;

        public HttpCorrelationIdMiddleware(RequestDelegate next,
            IOptions<HttpCorrelationIdOptions> options)
        {
            if (options == default)
            {
                throw new ArgumentNullException(nameof(options));
            }

            _options = options.Value;

            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext context,
            ICorrelationIdAccessor correlationIdAccessor)
        {
            context.TraceIdentifier = context.Request.Headers
                .TryGetValue(_options.StorageHeader, out var correlationId)
                ? correlationId
                : Guid.NewGuid().ToString();

            correlationIdAccessor.CorrelationId = context.TraceIdentifier;

            if (_options.IncludeInResponse)
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers.Add(_options.StorageHeader,
                        new[] {context.TraceIdentifier});
                    return Task.CompletedTask;
                });
            }

            await _next(context);
        }
    }
}