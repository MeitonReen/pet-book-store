using BookStore.Base.Implementations.HttpRequestService.Abstractions;
using BookStore.Base.Implementations.HttpRequestService.Helpers;
using BookStore.Base.Implementations.HttpRequestService.Helpers.Payload;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.HttpRequestService
{
    public class HttpRequestService : IHttpRequestService
    {
        private readonly ILogger<HttpRequestService> _logger;
        private HttpRequestServiceOptions _options;

        public HttpRequestService(ILogger<HttpRequestService> logger)
        {
            _logger = logger;
        }

        public void Configure(Action<HttpRequestServiceOptions> conf)
        {
            var options = new HttpRequestServiceOptions();
            conf?.Invoke(options);
            _options = options;

            _logger.LogDebug("CorrelationId: {@CorrelationId} HttpRequestService.Configure start",
                _options.CorrelationId);

            _options.AuthorityUrl = UrlHelper.CorrectSchemaUrl(_options.AuthorityUrl);
            _options.BaseUrl = UrlHelper.CorrectSchemaUrl(_options.BaseUrl);

            _logger.LogDebug(
                "CorrelationId: {@CorrelationId} HttpRequestService.Configure successfully completed, configured options: {@Options}",
                _options.CorrelationId, _options);
        }

        public HttpPayload Payload<TPayload>(TPayload payload) where TPayload : class =>
            new HttpPayloadClass<TPayload>(_options, payload, _logger);

        public HttpPayload Payload<TPayloadParamsItem>(params
            (string, TPayloadParamsItem)[] payloadParams) =>
            new HttpPayloadParams<TPayloadParamsItem>(_options, payloadParams, _logger);
    }
}