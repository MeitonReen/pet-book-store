namespace BookStore.Base.Implementations.__ObjectStorageUrlBuilder.Helpers
{
    public class ObjectStorageUrlConfigureOptions
    {
        public string ObjectStorageExternalPort { get; set; }
        public string AccessKey { get; set; }
        public string SecretKey { get; set; }
        public string BucketName { get; set; }
        public string ObjectName { get; set; }
        public int UrlExpires { get; set; }
    }
}