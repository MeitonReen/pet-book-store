namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IReadableDataFromResource<TData>
        where TData : class
    {
        TData? Read();

        Task<TData?> ReadAsync();

        IReadableDataFromResource<TData> ReadSettings(
            Func<IQueryable<TData>, IQueryable<TData>> readSettings);

        IReadableDataFromResource<TData> AddReadSettings(
            Func<IQueryable<TData>, IQueryable<TData>> readSettings);

        IReadableDataFromResource<TNewResultData> ReadSettings<TNewResultData>(
            Func<IQueryable<TData>, IQueryable<TNewResultData>> readSettings)
            where TNewResultData : class;

        IReadableDataFromResource<TNewResultData> AddReadSettings<TNewResultData>(
            Func<IQueryable<TData>, IQueryable<TNewResultData>> readSettings)
            where TNewResultData : class;
    }
}