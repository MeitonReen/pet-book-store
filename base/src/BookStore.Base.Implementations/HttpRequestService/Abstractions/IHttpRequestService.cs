using BookStore.Base.Implementations.HttpRequestService.Helpers;
using BookStore.Base.Implementations.HttpRequestService.Helpers.Payload;

namespace BookStore.Base.Implementations.HttpRequestService.Abstractions
{
    public interface IHttpRequestService
    {
        void Configure(Action<HttpRequestServiceOptions> conf);
        HttpPayload Payload<TPayload>(TPayload payload) where TPayload : class;

        HttpPayload Payload<TPayloadParamsItem>(params
            (string, TPayloadParamsItem)[] payloadParams);
    }
}