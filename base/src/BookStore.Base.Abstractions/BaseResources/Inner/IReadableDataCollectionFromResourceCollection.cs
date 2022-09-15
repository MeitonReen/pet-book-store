namespace BookStore.Base.Abstractions.BaseResources.Inner
{
    public interface IReadableDataCollectionFromResourceCollection<TDataCollectionItem>
        where TDataCollectionItem : class
    {
        IEnumerable<TDataCollectionItem> Read();

        ICollection<TDataCollectionItem> Read(
            Func<IQueryable<TDataCollectionItem>, ICollection<TDataCollectionItem>> executeSettings);

        List<TDataCollectionItem> Read(
            Func<IQueryable<TDataCollectionItem>, List<TDataCollectionItem>> executeSettings);

        Task<IEnumerable<TDataCollectionItem>> ReadAsync();

        Task<ICollection<TDataCollectionItem>> ReadAsync(
            Func<IQueryable<TDataCollectionItem>, Task<ICollection<TDataCollectionItem>>> executeSettings);

        Task<List<TDataCollectionItem>> ReadAsync(
            Func<IQueryable<TDataCollectionItem>, Task<List<TDataCollectionItem>>> executeSettings);

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