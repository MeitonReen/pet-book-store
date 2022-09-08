using BookStore.Base.Abstractions.BaseResources.Inner;
using Microsoft.EntityFrameworkCore;

namespace BookStore.Base.Implementations.BaseResources.Inner;

public class DataCollectionFromResourceCollectionByEfCore<TDataCollectionItem> :
    IReadableDataCollectionFromResourceCollection<TDataCollectionItem>
    where TDataCollectionItem : class
{
    private readonly IQueryable<TDataCollectionItem> _dataCollectionFromResourceCollection;

    private Func<IQueryable<TDataCollectionItem>, IQueryable<TDataCollectionItem>>
        _readSettings;

    public DataCollectionFromResourceCollectionByEfCore(
        IQueryable<TDataCollectionItem> dataCollectionFromResourceCollection)
    {
        _dataCollectionFromResourceCollection = dataCollectionFromResourceCollection;

        _readSettings = dataCollection => dataCollection;
    }

    public IEnumerable<TDataCollectionItem> Read() =>
        _readSettings(_dataCollectionFromResourceCollection);

    public async Task<IEnumerable<TDataCollectionItem>> ReadAsync() =>
        await _readSettings(_dataCollectionFromResourceCollection)
            .ToArrayAsync();

    public IReadableDataCollectionFromResourceCollection<TDataCollectionItem> ReadSettings(
        Func<IQueryable<TDataCollectionItem>,
            IQueryable<TDataCollectionItem>> readSettings)
    {
        _readSettings = readSettings;
        return this;
    }

    public IReadableDataCollectionFromResourceCollection<TDataCollectionItem> AddReadSettings(
        Func<IQueryable<TDataCollectionItem>, IQueryable<TDataCollectionItem>> readSettings)
    {
        _readSettings = readQuery => readSettings(
            _readSettings(readQuery));
        return this;
    }

    public IReadableDataCollectionFromResourceCollection<TNewResultData>
        ReadSettings<TNewResultData>(Func<IQueryable<TDataCollectionItem>,
            IQueryable<TNewResultData>> readSettings) where TNewResultData : class
        => new DataCollectionFromResourceCollectionByEfCore<TNewResultData>
        (readSettings(
            _readSettings(_dataCollectionFromResourceCollection)));

    public IReadableDataCollectionFromResourceCollection<TNewResultData>
        AddReadSettings<TNewResultData>(Func<IQueryable<TDataCollectionItem>,
            IQueryable<TNewResultData>> readSettings) where TNewResultData : class
        => new DataCollectionFromResourceCollectionByEfCore<TNewResultData>
        (readSettings(
            _readSettings(_dataCollectionFromResourceCollection)));
}