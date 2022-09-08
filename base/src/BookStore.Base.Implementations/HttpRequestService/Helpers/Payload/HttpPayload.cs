using Microsoft.Extensions.Logging;

namespace BookStore.Base.Implementations.HttpRequestService.Helpers.Payload
{
    public class HttpPayload
    {
        protected readonly ILogger _logger;
        protected HttpRequestServiceOptions _options;
        protected HttpRequestMessage _request = new();
        protected HttpSendManager _sendManager;

        public HttpPayload(HttpRequestServiceOptions options, ILogger logger)
        {
            _logger = logger;
            _options = options;

            _request.Method = options.HttpMethod;
            _request.RequestUri = new Uri(_options.BaseUrl + _options.PathToEndpoint);
            _sendManager = new HttpSendManager(_options, _request, _logger);
        }

        public virtual HttpSendManager ToBody => throw new NotImplementedException();
        public virtual HttpSendManager ToQuery => throw new NotImplementedException();
    }
}