using BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers.Configurator;

namespace BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Abstractions
{
    public interface IObjectStorageUrlBuilder
    {
        ObjectStorageUrlConfigurator Configurator { get; }
    }
}