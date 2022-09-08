namespace BookStore.Base.NonAttachedExtensions;

public static class QueryableExtensions
{
    public static IQueryable<TData> Share<TData>(
        this IQueryable<TData> targetBooksQuery,
        Action<IQueryable<TData>> queryConsumer)
    {
        queryConsumer(targetBooksQuery);
        return targetBooksQuery;
    }
}