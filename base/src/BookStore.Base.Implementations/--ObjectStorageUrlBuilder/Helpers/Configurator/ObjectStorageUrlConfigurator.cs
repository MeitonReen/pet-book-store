using BookStore.Base.Abstractions.ObjectResolver;
using BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers.Builder;
using BookStore.Base.Implementations.FluentApi;

namespace BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers.Configurator
{
    public class ObjectStorageUrlConfigurator : FluentApiBase
    {
        private readonly ObjectStorageUrlConfigureOptions _objectUrlConfigurationDTO = new();

        public ObjectStorageUrlConfigurator(IObjectResolver? objectResolver) : base(objectResolver)
        {
        }

        public ObjectStorageUrlBuilder Builder
        {
            get
            {
                _objectResolver.Register(_objectUrlConfigurationDTO);
                return new(_objectResolver);
            }
        }

        public ObjectStorageUrlConfigurator ObjectStorageExternalPort(string port)
        {
            _objectUrlConfigurationDTO.ObjectStorageExternalPort = port;
            return this;
        }

        public ObjectStorageUrlConfigurator Credentials(string accessKey, string secretKey)
        {
            _objectUrlConfigurationDTO.AccessKey = accessKey;
            _objectUrlConfigurationDTO.SecretKey = secretKey;
            return this;
        }

        public ObjectStorageUrlConfigurator ObjectLocation(string bucketName, string objectName)
        {
            _objectUrlConfigurationDTO.BucketName = bucketName;
            _objectUrlConfigurationDTO.ObjectName = objectName;
            return this;
        }

        public ObjectStorageUrlConfigurator UrlExpires(int urlExpires)
        {
            _objectUrlConfigurationDTO.UrlExpires = urlExpires;
            return this;
        }
    }
}