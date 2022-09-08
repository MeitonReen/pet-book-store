using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.FluentApi;
using BookStore.Base.Implementations.LoggingInterpolationSupport;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Minio;

namespace BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers.Builder
{
    public class ObjectStorageUrlBuilder : FluentApiBase
    {
        public ObjectStorageUrlBuilder(IObjectResolver? objectResolver) : base(objectResolver)
        {
        }

        public async Task<string> BuildAsync()
        {
            var logger = _objectResolver.Resolve<ILogger>();

            logger?.LogDebug($"{nameof(ObjectStorageUrlBuilder)} was started");

            var objectUrlConfigureOptions = _objectResolver.ResolveRequired<
                ObjectStorageUrlConfigureOptions>();
            var clientRequest = _objectResolver.ResolveRequired<IHttpContextAccessor>()
                ?.HttpContext?.Request;

            if (clientRequest == default)
            {
                throw new ArgumentException(
                    $"{nameof(ObjectStorageUrlBuilder)}." +
                    $"{nameof(clientRequest)} should be not default");
            }

            var referrerHost = clientRequest.GetTypedHeaders().Referer?.Host;

            logger?.LogDebug($"{nameof(referrerHost)}: {referrerHost}");

            var separatorPortIndex = referrerHost!.LastIndexOf(':');

            if (separatorPortIndex != -1)
            {
                referrerHost = referrerHost?[..separatorPortIndex];
            }

            var minioEndpoint = $"{referrerHost}:" +
                                $"{objectUrlConfigureOptions.ObjectStorageExternalPort}";

            logger?.LogDebug($"{nameof(minioEndpoint)}: {minioEndpoint}");

            var minioClient = new MinioClient(
                minioEndpoint,
                objectUrlConfigureOptions.AccessKey,
                objectUrlConfigureOptions.SecretKey);

            string urlToObject = await minioClient.PresignedGetObjectAsync(
                objectUrlConfigureOptions.BucketName,
                objectUrlConfigureOptions.ObjectName,
                objectUrlConfigureOptions.UrlExpires);

            logger?.LogDebug($"{nameof(ObjectStorageUrlBuilder)} successfully completed," +
                             $"Result: {urlToObject}");

            return urlToObject;
        }
    }
}