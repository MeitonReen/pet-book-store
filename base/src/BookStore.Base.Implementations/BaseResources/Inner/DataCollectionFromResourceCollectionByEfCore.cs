using System.Runtime.CompilerServices;
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

    public ICollection<TDataCollectionItem> Read() =>
        _readSettings(_dataCollectionFromResourceCollection)
            .ToList();

    public ICollection<TDataCollectionItem> Read(
        Func<IQueryable<TDataCollectionItem>, ICollection<TDataCollectionItem>> executeSettings)
        => executeSettings(
            _readSettings(
                _dataCollectionFromResourceCollection));
    public List<TDataCollectionItem> Read(
        Func<IQueryable<TDataCollectionItem>, List<TDataCollectionItem>> executeSettings)
        => executeSettings(
            _readSettings(
                _dataCollectionFromResourceCollection));
    
    public async Task<IEnumerable<TDataCollectionItem>> ReadAsync()
        => await _readSettings(
                _dataCollectionFromResourceCollection)
            .ToListAsync();

    public async Task<ICollection<TDataCollectionItem>> ReadAsync(
        Func<IQueryable<TDataCollectionItem>, Task<ICollection<TDataCollectionItem>>> executeSettings)
        => await executeSettings(
            _readSettings(
                _dataCollectionFromResourceCollection));

    public async Task<List<TDataCollectionItem>> ReadAsync(
        Func<IQueryable<TDataCollectionItem>, Task<List<TDataCollectionItem>>> executeSettings)
        => await executeSettings(
            _readSettings(
                _dataCollectionFromResourceCollection));

    IEnumerable<TDataCollectionItem> IReadableDataCollectionFromResourceCollection<TDataCollectionItem>.Read()
    {
        return Read();
    }

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

public static class TaskExtensions
{
    /// <summary>
    /// Casts the result type of the input task as if it were covariant
    /// </summary>
    /// <typeparam name="T">The original result type of the task</typeparam>
    /// <typeparam name="TResult">The covariant type to return</typeparam>
    /// <param name="task">The target task to cast</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static async Task<TResult> AsTask<T, TResult>(this Task<T> task)
        where T : TResult
        where TResult : class
    {
        return await task;
    }
}