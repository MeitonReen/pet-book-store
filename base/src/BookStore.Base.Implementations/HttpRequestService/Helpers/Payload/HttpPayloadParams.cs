using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.HttpRequestService.Helpers.Payload
{
    public class HttpPayloadParams<TPayloadParamsItem> : HttpPayload
    {
        private readonly (string, TPayloadParamsItem)[] _payload;

        public HttpPayloadParams(HttpRequestServiceOptions options,
            (string, TPayloadParamsItem)[] payload, ILogger logger)
            : base(options, logger)
        {
            _payload = payload;
        }

        public override HttpSendManager ToQuery
        {
            get
            {
                var payloadKeyValuePair = _payload.Select(item => new KeyValuePair<string, string>
                    (item.Item1, item.Item2.ToString()));

                var queryString = QueryString.Create(payloadKeyValuePair);

                _request.RequestUri = new Uri(_request.RequestUri.OriginalString + queryString);

                _logger.LogDebug("CorrelationId: {@CorrelationId} HttpPayloadParams<TPayloadParamsItem>.ToQuery get",
                    _options.CorrelationId);
                return _sendManager;
            }
        }
    }
}