namespace BookStore.Base.Implementations.HttpRequestService.Helpers
{
    public class HttpRequestServiceOptions
    {
        public string BaseUrl { get; set; }
        public string PathToEndpoint { get; set; }
        public HttpMethod HttpMethod { get; set; }
        public string AuthorityUrl { get; set; }
        public string Audience { get; set; }
        public string Scope { get; set; }
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string CorrelationId { get; set; }
        public string CorrelationIdStorageHttpHeaderName { get; set; }
    }
}