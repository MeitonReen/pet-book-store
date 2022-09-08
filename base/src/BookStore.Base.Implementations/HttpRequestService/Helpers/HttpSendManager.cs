using System.Net.Http.Json;
using System.Text;
using BookStore.Base.Contracts.Implementations;
using BookStore.Base.Contracts.Implementations.AccessToken.V1_0_0;
using BookStore.Base.Implementations.__Obsolete;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using static IdentityModel.OidcConstants;

namespace BookStore.Base.Implementations.HttpRequestService.Helpers
{
    public class HttpSendManager
    {
        private readonly HttpClient _client = new();

        private readonly ILogger _logger;
        private readonly HttpRequestServiceOptions _options;
        private readonly HttpRequestMessage _requestMessage;

        public HttpSendManager(HttpRequestServiceOptions options,
            HttpRequestMessage requestMessage, ILogger logger)
        {
            _logger = logger;

            if (options == default)
            {
                throw new ArgumentNullException(nameof(HttpRequestServiceOptions));
            }

            _options = options;
            _requestMessage = requestMessage;

            _client.BaseAddress = new Uri(_options.BaseUrl);
        }

        public async Task<TResponse?> SendAsync<TResponse>()
        {
            _logger.LogDebug("CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> start",
                _options.CorrelationId);

            _logger.LogDebug(
                "CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> access token request sending...",
                _options.CorrelationId);
            var accessTokenResponse = await GetAccessTokenAsync();

            if (accessTokenResponse?.access_token == default)
            {
                _logger.LogDebug(
                    "CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> receiving an access token fails",
                    _options.CorrelationId);
                return default(TResponse);
            }

            _logger.LogDebug(
                "CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> access token successfully received, response: {@Response}",
                _options.CorrelationId, accessTokenResponse);

            _requestMessage.Headers.Add(HeaderNames.Authorization,
                $"{accessTokenResponse.token_type} {accessTokenResponse.access_token}");

            _logger.LogDebug("CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> options: {@Options}",
                _options.CorrelationId, _options);

            if (_options.CorrelationIdStorageHttpHeaderName != default &&
                _options.CorrelationId != default)
            {
                _requestMessage.Headers.Add(_options.CorrelationIdStorageHttpHeaderName,
                    _options.CorrelationId);
            }

            _logger.LogDebug(
                "CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> target request sending...",
                _options.CorrelationId);
            var response = await _client.SendAsync(_requestMessage);
            _logger.LogDebug(
                "CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> target data successfully received, response: {@Response}",
                _options.CorrelationId, response);

            var responseToResult = await response.Content.ReadFromJsonAsync<TResponse>();
            _logger.LogDebug(
                "CorrelationId: {@CorrelationId} HttpSendManager.SendAsync<TResponse> response as target type to result: {@Result}",
                _options.CorrelationId, responseToResult);

            return responseToResult;
        }

        private async Task<AccessTokenResponse?> GetAccessTokenAsync()
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Post,
                RequestUri = new Uri(_options.AuthorityUrl + Constants.Authorization.TokenEndpoint),
                Content = new StringContent(
                    $"{TokenRequest.GrantType}={GrantTypes.ClientCredentials}&" +
                    $"{TokenRequest.ClientId}={_options.ClientId}&" +
                    $"{TokenRequest.ClientSecret}={_options.ClientSecret}&" +
                    $"{TokenRequest.Scope}={_options.Scope}",
                    Encoding.UTF8, MimeTypes.Application.XWWWFormUrlencoded)
            };

            var response = await _client.SendAsync(request);
            return await response.Content.ReadFromJsonAsync<AccessTokenResponse>();
        }
    }
}