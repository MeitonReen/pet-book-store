namespace BookStore.Base.Implementations
{
    public static class UrlHelper
    {
        public static string CorrectSchemaUrl(string url)
        {
            return !url.StartsWith("http://") ? $"http://{url}" : url;
        }
    }
}