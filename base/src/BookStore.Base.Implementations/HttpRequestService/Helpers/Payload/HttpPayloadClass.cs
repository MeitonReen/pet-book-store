using System.Net.Http.Json;
using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.HttpRequestService.Helpers.Payload
{
    public class HttpPayloadClass<TPayload> : HttpPayload where TPayload : class
    {
        private readonly TPayload _payload;

        public HttpPayloadClass(HttpRequestServiceOptions options, TPayload payload,
            ILogger logger)
            : base(options, logger)
        {
            _payload = payload;
        }

        public override HttpSendManager ToBody
        {
            get
            {
                _request.Content = JsonContent.Create(_payload);

                _logger.LogDebug("CorrelationId: {@CorrelationId} HttpPayloadClass<TPayload>.ToBody get",
                    _options.CorrelationId);
                return _sendManager;
            }
        }
    }
}