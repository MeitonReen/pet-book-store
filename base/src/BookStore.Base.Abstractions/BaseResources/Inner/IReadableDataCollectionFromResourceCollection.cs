namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IReadableDataCollectionFromResourceCollection<TDataCollectionItem>
        where TDataCollectionItem : class
    {
        IEnumerable<TDataCollectionItem> Read();

        Task<IEnumerable<TDataCollectionItem>> ReadAsync();

        IReadableDataCollectionFromResourceCollection<TDataCollectionItem> ReadSettings(
            Func<IQueryable<TDataCollectionItem>, IQueryable<TDataCollectionItem>> readSettings);

        IReadableDataCollectionFromResourceCollection<TDataCollectionItem> AddReadSettings(
            Func<IQueryable<TDataCollectionItem>, IQueryable<TDataCollectionItem>> readSettings);

        IReadableDataCollectionFromResourceCollection<TNewResultData> ReadSettings<TNewResultData>(
            Func<IQueryable<TDataCollectionItem>, IQueryable<TNewResultData>> readSettings)
            where TNewResultData : class;

        IReadableDataCollectionFromResourceCollection<TNewResultData> AddReadSettings<TNewResultData>
            (Func<IQueryable<TDataCollectionItem>, IQueryable<TNewResultData>> readSettings)
            where TNewResultData : class;
    }
}