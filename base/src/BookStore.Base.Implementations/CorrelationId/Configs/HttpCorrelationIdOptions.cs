using BookStore.Base.Implementations.__Obsolete;

namespace BookStore.Base.Implementations.CorrelationId.Configs
{
    public class HttpCorrelationIdOptions
    {
        private const string StorageHeaderDefault =
            Constants.CorrelationId.StorageHeaderDefault;

        public string StorageHeader { get; set; } = StorageHeaderDefault;
        public bool IncludeInResponse { get; set; } = false;
    }
}