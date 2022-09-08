namespace BookStore.Base.NonAttachedExtensions;

public static class ObjectExtensions
{
    public static T ShareTo<T>(this T entity, Action<T> shareToSettings)
    {
        shareToSettings(entity);
        return entity;
    }
}